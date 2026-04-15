using DownloadComics.models;
using DownloadComics.services;
using DownloadComics.utilities;
using FuzzierSharp;
using System.IO;
using System.IO.Pipes;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DownloadComics.windows
{
    /// <summary>
    /// Logique d'interaction pour RestoreBackup.xaml
    /// </summary>
    public partial class RestoreBackup : Window
    {
        private readonly static CancellationTokenSource cancellationTokenSource = new();
        private const string pipeName = "SearchComicsPipe";
        private readonly JdownloaderService jdownloaderService;

        public RestoreBackup()
        {
            InitializeComponent();
            jdownloaderService = JdownloaderService.Instance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var dir in Directory.GetDirectories(FileService.BackupDirPath))
            {
                folderTree.Items.Add(BuildTreeIterative(dir));
            }
        }

        private static TreeViewItem BuildTreeIterative(string rootPath)
        {
            var root = new TreeViewItem
            {
                Tag = rootPath,
                Header = Path.GetFileName(rootPath)
            };

            var stack = new Stack<(TreeViewItem node, string path)>();
            stack.Push((root, rootPath));

            while (stack.Count > 0)
            {
                var (node, path) = stack.Pop();

                string[] subdirs;
                try
                {
                    subdirs = Directory.GetDirectories(path);
                }
                catch (Exception)
                {
                    continue;
                }

                foreach (var sd in subdirs)
                {
                    var child = new TreeViewItem
                    {
                        Tag = sd,
                        Header = Path.GetFileName(sd)
                    };

                    node.Items.Add(child);
                    stack.Push((child, sd));
                }
            }

            return root;
        }

        private static TreeViewItem AddFolder(string path)
        {
            TreeViewItem item = new()
            {
                Tag = path,
                Header = $"{Path.GetFileName(path)}"
            };
            foreach (var dir in Directory.GetDirectories(path))
            {
                item.Items.Add(AddFolder(dir));
            }

            return item;

        }

        private async void RestoreBtn_Click(object sender, RoutedEventArgs e)
        {
            if (folderTree.SelectedItem is not TreeViewItem item)
                return;

            var pathObj = item.Tag;
            if (pathObj is null)
                return;

            string sourcePath = pathObj.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(sourcePath) || !Directory.Exists(sourcePath))
                return;


            bool previousIsEnabled = folderTree.IsEnabled;
            folderTree.IsEnabled = false;

            try
            {
                string destPath = sourcePath.Replace(FileService.BackupDirPath, FileService.ComicsDirectory);
                if (Directory.Exists(destPath))
                    Directory.Delete(destPath, true);

                Directory.Move(sourcePath, destPath);

                UpdateTree(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur pendant la restauration : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                folderTree.IsEnabled = previousIsEnabled;
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (folderTree.SelectedItem is TreeViewItem item)
            {
                var path = item.Tag?.ToString();
                if (path != null && IsDownloaded(path))
                {
                    UpdateTree(item);
                }
            }
        }

        private void UpdateTree(DependencyObject item)
        {
            if (item is TreeViewItem currentItem)
            {
                var parent = currentItem.Parent;
                DeleteTreeItem(currentItem);
                if (parent is TreeViewItem parentItem && parentItem.Items.Count == 0)
                {
                    UpdateTree(parentItem);
                }
            }
        }

        private void DeleteTreeItem(TreeViewItem item)
        {
            var parent = ItemsControl.ItemsControlFromItemContainer(item);
            if (parent != null)
            {
                parent.Items.Remove(item);

                var path = item.Tag?.ToString();
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
            else
            {
                folderTree.Items.Remove(item);
            }
        }

        private static bool IsDownloaded(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            string comicPath = path.Replace(FileService.BackupDirPath, FileService.ComicsDirectory);
            return (Directory.Exists(comicPath) && PathControl.CountPage(comicPath) > PathControl.CountPage(path))
                || ContainDouble(path);
        }

        private static bool ContainDouble(string path)
        {
            return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                .GroupBy(Path.GetFileNameWithoutExtension).Any(group => group.Any());

        }

        private async void SearchBTN_Click(object sender, RoutedEventArgs e)
        {
            if (folderTree.SelectedItem is TreeViewItem item)
            {
                string? tag = item.Tag.ToString();
                string? header = item.Header.ToString();
                if (tag != null && header != null)
                {
                    string author = tag.Replace(FileService.BackupDirPath, string.Empty)[1..].Split(Path.DirectorySeparatorChar).First();
                    string comic = header;

                    string? jd = await jdownloaderService.GetComicJdownloader(author, comic);
                    ComicUtility.SearchComic(author, comic, FileService.BackupDirPath, "true", jd);

                    await StartSearchPipe(Dispatcher);
                }



            }
        }

        private async Task StartSearchPipe(Dispatcher dispatcher)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await using NamedPipeServerStream pipeServer = new(pipeName, PipeDirection.InOut, 1,
                        PipeTransmissionMode.Message, PipeOptions.Asynchronous);

                    await pipeServer.WaitForConnectionAsync(cancellationTokenSource.Token);

                    using StreamReader reader = new(pipeServer);
                    await using StreamWriter writer = new(pipeServer) { AutoFlush = true };

                    while (pipeServer.IsConnected)
                    {
                        string? request = await reader.ReadLineAsync();
                        if (request == null) break;

                        await dispatcher.InvokeAsync(() =>
                        {
                            if (request.Equals(ConsoleKey.Y.ToString(), StringComparison.OrdinalIgnoreCase))
                            {
                                if (folderTree.SelectedItem is TreeViewItem item)
                                {
                                    var path = item.Tag?.ToString();
                                    if (path != null && IsDownloaded(path))
                                    {
                                        UpdateTree(item);
                                    }
                                }
                            }
                        });
                    }
                }
                catch (OperationCanceledException)
                {

                }
            });
        }
    }
}
