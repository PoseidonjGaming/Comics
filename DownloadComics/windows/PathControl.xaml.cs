using DownloadComics.models;
using DownloadComics.resources.path;
using DownloadComics.services;
using DownloadComics.utilities;
using FuzzierSharp;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace DownloadComics.windows
{
    /// <summary>
    /// Logique d'interaction pour PathControl.xaml
    /// </summary>
    public partial class PathControl : Window
    {
        

        private readonly Comic comic;
        private readonly Options? options;
        private CancellationTokenSource? _scanCts;
        public ObservableCollection<string> Paths { get; } = [];


        public PathControl(Comic comic)
        {
            InitializeComponent();

            this.comic = comic;
            DataContext = this;
            pathList.ItemsSource = Paths;

            options = JsonConvert.DeserializeObject<Options>(Properties.Settings.Default.Options);

            if (options != null)
            {
                foreach (var path in options.Paths)
                {
                    if (!string.IsNullOrEmpty(path))
                        rootCB.Items.Add(path);
                }

                if (rootCB.Items.Count > 0)
                    rootCB.SelectedIndex = 0;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_scanCts != null && !_scanCts.IsCancellationRequested)
            {
                _scanCts.Cancel();
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _scanCts = new CancellationTokenSource();

            bool previousIsEnabled = IsEnabled;
            try
            {
                IsEnabled = false;
                Mouse.OverrideCursor = Cursors.Wait;

                await Task.Run(() => Scan(_scanCts.Token)).ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                Mouse.OverrideCursor = null;
                IsEnabled = previousIsEnabled;
            }
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {

            if (rootCB.SelectedItem is string root)
            {
                comic.Path = Path.Combine(root, pathTXT.Text.Trim());
                if (pathList.SelectedItem != null)
                {
                    try
                    {
                        if (Directory.Exists(comic.Path))
                        {
                            string destPath = comic.Path.Replace(root, FileService.BackupDirPath);

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
                                Directory.Move(comic.Path, destPath);
                            }
                        }
                        DialogResult = true;

                    }
                    catch (IOException ex)
                    {
                        DialogResult = false;
                        MessageBox.Show($"{PathStrings.Msg_Delete_Folder_Error} : {ex.Message}");
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        DialogResult = false;
                        MessageBox.Show($"{PathStrings.Msg_Folder_Access_Error} : {ex.Message}");
                    }

                }

            }
            else if (pathList.Items.Count > 0)
            {
                MessageBox.Show(PathStrings.Msg_Path_Not_Selected);
                DialogResult = false;
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void Scan(CancellationToken ct)
        {
            try
            {
                foreach (string root in rootCB.Items)
                {
                    ct.ThrowIfCancellationRequested();

                    var author = Process.ExtractOne(comic.Author,
                        Directory.EnumerateDirectories(root), s=>Path.GetFileName(s).ToLower());

                    if (author != null && author.Score == 100)
                    {
                        IEnumerable<string> comics = Directory.EnumerateDirectories(author.Value, "*", SearchOption.AllDirectories);
                        var comicsResult = Process.ExtractOne(comic.PackageName, comics, s => Path.GetFileName(s).ToLower());

                        //cas 1 -> comics exact
                        if (comicsResult != null && comicsResult.Score == 100)
                        {
                            if (comic.NumberPages > CountPage(comicsResult.Value))
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    Paths.Add(comicsResult.Value.Replace(root, string.Empty)[1..]);
                                });
                            }
                            else
                            {
                                MessageBox.Show(PathStrings.Msg_Comic_Not_Found);
                                Dispatcher.Invoke(() => DialogResult = false);
                            }
                        }
                        else
                        {
                            //cas 2 -> comics absant mais avec chapitre precedent
                            FindPreviousChapter(comic.PackageName, comics, root);
                        }
                    }

                    //cas 3 -> author absant
                    if (Paths.Count == 0)
                    {
                        Dispatcher.Invoke(() =>
                        {

                            if (RegexUtility.ChapterRegex().IsMatch(comic.PackageName))
                            {
                                string comicName = RegexUtility.ChapterRegex().Replace(comic.PackageName, "").Trim();
                                Paths.Add(Path.Combine(comic.Author, comicName, comic.PackageName));
                            }
                            else
                                Paths.Add(Path.Combine(comic.Author, comic.PackageName));
                        });
                    }
                }
            }
            catch
            {

            }

        }



        private void FindPreviousChapter(string comic, IEnumerable<string> comics, string root)
        {
            if (int.TryParse(RegexUtility.ChapterRegex().Match(comic).Value, out int numChapter))
            {
                numChapter--;

                string prevChapter = RegexUtility.ChapterRegex().Replace(comic, numChapter.ToString());
                var comicsResult = Process.ExtractOne(prevChapter, comics, Path.GetFileName, cutoff: 100);

                if (comicsResult != null)
                {

                    Dispatcher.Invoke(() =>
                    {
                        Paths.Add(comicsResult.Value.Replace(root, string.Empty)
                            .Replace(prevChapter, this.comic.PackageName)[1..]);
                    });
                }
                else if (numChapter >= 1)
                {
                    FindPreviousChapter(prevChapter, comics, root);
                }
            }
        }

        private void PathList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (pathList.SelectedItem is string selectedPath)
            {
                pathTXT.Text = selectedPath;
            }
            else
            {
                pathTXT.Text = string.Empty;
            }
        }

        public static int CountPage(string path)
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
                    using (var en = Directory.EnumerateFiles(current).GetEnumerator())
                    {
                        while (en.MoveNext())
                            pages++;
                    }

                    foreach (var dir in Directory.EnumerateDirectories(current))
                        dirs.Push(dir);
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
            return pages;
        }


    }
}
