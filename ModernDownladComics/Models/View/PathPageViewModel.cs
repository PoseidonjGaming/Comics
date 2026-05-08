using ComicsInfraLib.Helpers;
using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FuzzierSharp;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModernDownladComics.Models.View
{
    public partial class PathPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private partial Comic Comic { get; set; }
        public ObservableCollection<string> Roots { get; set; } = [];
        [ObservableProperty]
        public partial string SelectedRoot { get; set; }
        public ObservableCollection<string> Paths { get; set; }
        [ObservableProperty]
        public partial string SelectedPath { get; set; }

        private readonly ISettingsService _settingsService;
        private readonly IPathService _pathService;
        private CancellationTokenSource? _scanCts;
        public Type? ReturnType;

        public event Action<Type>? NavigateEvent;
        public event Action<ObservableCollection<string>, string>? AddPathEvent;

        private static AppState State => AppStateStore.Instance;
        public PathPageViewModel(ISettingsService settingsService, IPathService pathService)
        {
            _settingsService = settingsService;
            _pathService = pathService;
            Roots = new ObservableCollection<string>(settingsService.GetOptions().Paths);
            SelectedRoot = Roots.First();
            Paths = [];
            SelectedPath = "";
            Comic = new();
        }

        public void Init(PathPageArgs args)
        {
            if (args.Comic != null)
                Comic = args.Comic;
            ReturnType = args.ReturnType;
        }

        [RelayCommand]
        public void Select()
        {
            if (ReturnType == null || NavigateEvent == null) return;
            Comic.Path = Path.Combine(SelectedRoot, SelectedPath.Trim());

            try
            {
                /*if (Directory.Exists(Comic.Path))
                {
                    string destPath = Comic.Path.Replace(SelectedRoot,
                        _pathService.BackupDirPath);

                    DirectoryInfo? destInfo = Directory.GetParent(destPath);

                    if (destInfo is not null)
                    {
                        if (!destInfo.Exists)
                        {
                            destInfo.Create();
                        }
                    }

                    if (_pathService.BackupDirPath[0] == SelectedRoot[0])
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

                FileUtility.WriteFile<List<Comic>>(_pathService.BackupFilePath,
                   [.. State.Comics]);
                FileUtility.WriteFile<List<Track>>(_pathService.TrackFilePath,
                    State.Tracks);

                NavigateEvent(ReturnType);
            }
        }
        [RelayCommand]
        public void Cancel()
        {
            if (ReturnType == null || NavigateEvent == null) return;
            NavigateEvent(ReturnType);
        }

        public async void Load(Func<bool> getPreviousState, Action<bool> setState)
        {
            _scanCts = new CancellationTokenSource();
            bool previousIsEnabled = getPreviousState();
            try
            {
                setState(false);

                await Task.Run(() => Scan(_scanCts.Token)).ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                setState(previousIsEnabled);
            }
        }

        public void Unload()
        {
            if (_scanCts != null && !_scanCts.IsCancellationRequested)
            {
                _scanCts.Cancel();
            }
        }

        private async void Scan(CancellationToken ct)
        {
            if (Comic != null && AddPathEvent != null)
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
                                    Paths.Add(comicsResult.Value.Replace(root, string.Empty)[1..]);
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
                        if (RegexUtility.ChapterRegex().IsMatch(Comic.PackageName))
                        {
                            string comicName = RegexUtility.ChapterRegex()
                            .Replace(Comic.PackageName, "").Trim();
                            Paths.Add(Path.Combine(Comic.Author, comicName, Comic.PackageName));
                        }
                        else
                        {
                            AddPathEvent(Paths, Path.Combine(Comic.Author, Comic.PackageName));
                        }
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
                    Paths.Add(comicsResult.Value.Replace(root, string.Empty)
                    .Replace(prevChapter, Comic.PackageName)[1..]);
                }
                else if (numChapter >= 1)
                {
                    FindPreviousChapter(prevChapter, comics, root);
                }
            }
        }
    }
}