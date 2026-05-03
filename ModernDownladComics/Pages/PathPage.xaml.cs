using ComicsLib.Models;
using ComicsLib.Services;
using ComicsLib.Utilities;
using FuzzierSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ModernDownladComics.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PathPage : Page
    {
        public Comic? Comic { get; set; }
        public ObservableCollection<string> Roots = [];
        public ObservableCollection<string> Paths = [];
        private readonly ISettingsService settingsService;
        private CancellationTokenSource? _scanCts;
        private Type? _typePage;
        private static AppState State => AppStateStore.Instance;
        public PathPage()
        {
            InitializeComponent();

            settingsService = App.Services.GetRequiredService<ISettingsService>();
            foreach (var path in settingsService.GetOptions().Paths)
            {
                Roots.Add(path);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is PathPageArgs args)
            {
                Comic = args.Comic;
                _typePage = args.ReturnType;
            }
        }

        private void SelectBTN_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (rootCMB.SelectedItem is string root && Comic != null)
            {
                Comic.Path = Path.Combine(root, pathTXT.Text.Trim());

                if (pathLST.SelectedItem != null)
                {
                    try
                    {
                        /*if (Directory.Exists(Comic.Path))
                        {
                            string destPath = Comic.Path.Replace(root, FileService.BackupDirPath);

                            DirectoryInfo? destInfo = Directory.GetParent(destPath);

                            if (destInfo is not null)
                            {
                                if (!destInfo.Exists)
                                {
                                    destInfo.Create();
                                }
                            }

                            if (FileService.BackupDirPath[0] == root[0])
                            {
                                Directory.Move(Comic.Path, destPath);
                            }
                        }*/
                    }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                    finally
                    {
                        State.Comics.Add(Comic);
                        State.AddTrack(new(Comic.BaseURL, Comic.URL, Comic.Host));
                        Frame.Navigate(_typePage);
                    }
                }
            }
        }

        private void CancelBTN_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddPage));
        }

        private async void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            _scanCts = new CancellationTokenSource();
            bool previousIsEnabled = IsEnabled;
            try
            {
                IsEnabled = false;

                await Task.Run(() => Scan(_scanCts.Token)).ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                IsEnabled = previousIsEnabled;
            }
        }

        private async void Scan(CancellationToken ct)
        {
            if (Comic != null)
            {
                try
                {
                    foreach (string root in Roots)
                    {
                        ct.ThrowIfCancellationRequested();

                        var author = Process.ExtractOne(Comic.Author,
                            Directory.EnumerateDirectories(root), s => Path.GetFileName(s).ToLower());

                        if (author != null && author.Score == 100)
                        {
                            IEnumerable<string> comics = Directory.EnumerateDirectories(author.Value, "*", SearchOption.AllDirectories);
                            var comicsResult = Process.ExtractOne(Comic.PackageName, comics, s => Path.GetFileName(s).ToLower());

                            //cas 1 -> comics exact
                            if (comicsResult != null && comicsResult.Score == 100)
                            {
                                if (Comic.NumberPages > CountPage(comicsResult.Value))
                                {
                                    DispatcherQueue.TryEnqueue(() =>
                                    {
                                        Paths.Add(comicsResult.Value.Replace(root, string.Empty)[1..]);
                                    });
                                }
                                else
                                {
                                    ContentDialog dialog = new()
                                    {
                                        Title = "Alert",
                                        Content = "No comcis found",
                                        PrimaryButtonText = "Ok"
                                    };
                                    await dialog.ShowAsync();
                                }
                            }
                            else
                            {
                                //cas 2 -> comics absant mais avec chapitre precedent
                                FindPreviousChapter(Comic.PackageName, comics, root);
                            }
                        }
                    }

                    if (Paths.Count == 0)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            if (RegexUtility.ChapterRegex().IsMatch(Comic.PackageName))
                            {
                                string comicName = RegexUtility.ChapterRegex()
                                .Replace(Comic.PackageName, "").Trim();
                                Paths.Add(Path.Combine(Comic.Author, comicName, Comic.PackageName));
                            }
                            else
                            {
                                Paths.Add(Path.Combine(Comic.Author, Comic.PackageName));
                            }
                        });
                    }
                }
                catch { }
            }
        }

        private static int CountPage(string path)
        {
            if (!Directory.Exists(path))
                return 0;

            int pages = 0;
            var dirs = new Stack<string>();
            dirs.Push(path);

            while (dirs.Count > 0)
            {
                var current = dirs.Pop();
                try
                {
                    using (var en = Directory.EnumerateDirectories(current).GetEnumerator())
                    {
                        while (en.MoveNext())
                            pages++;
                    }

                    foreach (var dir in Directory.EnumerateDirectories(current))
                    {
                        dirs.Push(dir);
                    }
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
            return pages;
        }

        private void FindPreviousChapter(string comic, IEnumerable<string> comics, string root)
        {
            if (int.TryParse(RegexUtility.ChapterRegex().Match(comic).Value, out int numChapter)
                && Comic != null)
            {
                numChapter--;

                string prevChapter = RegexUtility.ChapterRegex().Replace(comic, numChapter.ToString());
                var comicsResult = Process.ExtractOne(prevChapter, comics, Path.GetFileName, cutoff: 100);

                if (comicsResult != null)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    Paths.Add(comicsResult.Value.Replace(root, string.Empty)
                    .Replace(prevChapter, Comic.PackageName)));
                }
                else if (numChapter >= 1)
                {
                    FindPreviousChapter(prevChapter, comics, root);
                }
            }
        }

        private void Page_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (_scanCts != null && !_scanCts.IsCancellationRequested)
            {
                _scanCts.Cancel();
            }
        }
    }
}
