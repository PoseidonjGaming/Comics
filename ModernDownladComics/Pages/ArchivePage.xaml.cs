using ComicsLib.Services;
using FuzzierSharp.Extractor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SearchComicsLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArchivePage : Page
    {
        public ObservableCollection<TreeItem> Items { get; set; } = [];
        public TreeItem? SelectedItem { get; set; }

        private readonly JdownloaderService jdownloaderService;
        private readonly ICredentialsService credentialsService;
        public ArchivePage()
        {
            InitializeComponent();

            jdownloaderService = App.Services.GetRequiredService<JdownloaderService>();
            credentialsService = App.Services.GetRequiredService<ICredentialsService>();


        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var dir in Directory.GetDirectories(FileService.BackupDirPath))
            {
                Items.Add(BuildTree(dir));
            }
        }

        private static TreeItem BuildTree(string dir)
        {
            TreeItem item = new(dir);

            var stack = new Stack<(TreeItem node, string path)>();
            stack.Push((item, dir));

            while (stack.Count > 0)
            {
                var (node, path) = stack.Pop();

                string[] subDirs;
                try
                {
                    subDirs = Directory.GetDirectories(path);
                }
                catch (Exception) { continue; }

                foreach (var subDir in subDirs)
                {
                    TreeItem child = new(subDir, node);
                    node.Children.Add(child);
                    stack.Push((child, subDir));
                }
            }

            return item;
        }

        private async void SearchBTN_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null) return;

            string? jd = await jdownloaderService.GetComicJdownloader(SelectedItem.GetAuthor(),
                SelectedItem.Name);

            ContentDialog dialog = new()
            {
                Title = "Search",
                Content = new SearchPage(AddToPanel(FileService.ComicsDirectory, "Manga"),
                    AddToPanel(FileService.BackupDirPath, "Backup"), jd ?? "Download not found",
                    $"Do you want to delete the backup of {SelectedItem.Name}"),
                PrimaryButtonText = "Yes",
                DefaultButton = ContentDialogButton.Primary,
                CloseButtonText = "No",
                XamlRoot = this.XamlRoot
            };
            ContentDialogResult dialogRes = await dialog.ShowAsync();

            if (dialogRes == ContentDialogResult.Primary)
            {
                DeleteItem();
            }
        }

        private string AddToPanel(string path, string from)
        {
            if (SelectedItem == null)
                return "null";
            string authorPath = SearchUtility.GetAuthorPath(SelectedItem.GetAuthor(), path);

            IEnumerable<ExtractedResult<string>> results = SearchUtility.GetComics(authorPath,
                SelectedItem.Name);
            ExtractedResult<string>? res = results.FirstOrDefault();

            if (res == null)
                return "Empty";
           
            string fromPath = res.Value.Replace(authorPath, string.Empty)[1..];

            return $"From {from}: {SearchUtility.CountPage(res.Value)} pages - {fromPath}";
        }

        private static TextBlock CreateTextBlock(string text)
        {
            return new()
            {
                Text = text,
                FontSize = 16
            };
        }
        private async void RestoreBTN_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                ContentDialog dialog = new()
                {
                    Title = "Restore",
                    Content = "Do you want to restore this backup?",
                    DefaultButton = ContentDialogButton.Primary,
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No",
                    XamlRoot = this.XamlRoot
                };

                ContentDialogResult res = await dialog.ShowAsync();
                if (res == ContentDialogResult.Primary)
                {
                    string destPath = SelectedItem.Path.Replace(FileService.BackupDirPath,
                        FileService.ComicsDirectory);
                    Directory.Delete(destPath, true);
                    Directory.Move(SelectedItem.Path, destPath);
                    DeleteItem();

                }
            }

        }

        private void DeleteItem()
        {
            if (SelectedItem != null)
            {
                TreeItem item = SelectedItem;
                if (SelectedItem.Parent == null)
                {
                    Items.Remove(SelectedItem);
                }
                else if (SelectedItem.Remove() && item.Parent != null)
                {
                    Items.Remove(item.Parent);
                    Directory.Delete(item.Parent.Path, true);
                }
            }

        }

        private void DeleteBTN_Click(object sender, RoutedEventArgs e)
        {
            DeleteItem();
        }
    }

    public class TreeItem(string path, TreeItem? parent = null)
    {
        public string Name = System.IO.Path.GetFileName(path);
        public string Path = path;
        public TreeItem? Parent = parent;
        public ObservableCollection<TreeItem> Children { get; set; } = [];

        public string GetAuthor()
        {
            return Path.Replace(FileService.BackupDirPath, string.Empty)[1..]
                .Split(System.IO.Path.DirectorySeparatorChar).First();
        }

        public bool Remove()
        {
            if (Parent != null)
            {
                Parent.Children.Remove(this);
                if (Directory.Exists(Path))
                    Directory.Delete(Path, true);
                if (Parent.Children.Count == 0)
                {
                    return Parent.Remove();
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }

        }
    }
}
