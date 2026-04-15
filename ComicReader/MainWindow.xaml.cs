using ComicReader.model;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ComicReader
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private ObservableCollection<FolderDir> dirs = [];
        private readonly string comicsPath = @"E:\Manga Scan\Manga\hentai";
        private List<string> comicsImages = [];
        private int currentImageIndex = -1;
        private bool isFrontVisible = true;

        public MainWindow()
        {
            InitializeComponent();
            LoadFolder(comicsPath);
        }

        private async void FolderListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (folderListView.SelectedItem is FolderDir dir)
            {
                LoadFolder(dir.Path);
                UpdateBreadcrumb(dir.Path);
            }


        }

        private void LoadFolder(string path)
        {
            dirs.Clear();
            foreach (var item in Directory.EnumerateDirectories(path))
            {
                dirs.Add(new(item));
            }

            if (dirs.Count == 0)
                CollapseFoldersPane();
            else
                EnsureFoldersPaneVisible();

            SetImageList(path);
            ShowImageAt(0);
            UpdateBreadcrumb(path);
        }

        private void CollapseFoldersPane()
        {
            folderListView.Visibility = Visibility.Collapsed;
            FoldersColumn.Width = new GridLength(0);
            showFoldersButton.Visibility = Visibility.Visible;

            imageHost.HorizontalAlignment = HorizontalAlignment.Center;
            imageHost.VerticalAlignment = VerticalAlignment.Center;
        }

        private void EnsureFoldersPaneVisible()
        {
            folderListView.Visibility = Visibility.Visible;
            FoldersColumn.Width = new GridLength(415);
            showFoldersButton.Visibility = Visibility.Collapsed;

            imageHost.HorizontalAlignment = HorizontalAlignment.Stretch;
            imageHost.VerticalAlignment = VerticalAlignment.Stretch;
        }

        private void ShowFoldersButton_Click(object sender, RoutedEventArgs e)
        {
            EnsureFoldersPaneVisible();
            comicsImages.Clear();
            LoadFolder(comicsPath);
        }

        private void SetImageList(string folderPath)
        {
            comicsImages.Clear();
            currentImageIndex = -1;

            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return;

            var extensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };

            try
            {
                comicsImages = [.. Directory
                    .EnumerateFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => extensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                    .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)];

                if (comicsImages.Count > 0)
                    currentImageIndex = 0;
            }
            catch
            {
                comicsImages = [];
                currentImageIndex = -1;
            }
        }

        private void ShowImageAt(int index)
        {
            if (index < 0 || index >= comicsImages.Count)
            {
                comicsImage.Source = null;
                return;
            }

            var file = comicsImages[index];
            var uri = new Uri("file:///" + file.Replace('\\', '/'));

            var targetImage = isFrontVisible ? comicsImageBack : comicsImage;
            var visibleImage = isFrontVisible ? comicsImage : comicsImageBack;

            visibleImage.Visibility = Visibility.Visible;
            targetImage.Visibility = Visibility.Visible;

            var bmp = new BitmapImage();

            int expectedIndex = index;
            string expectedPath = file;

            bmp.ImageOpened += (s, e) =>
            {
                try
                {
                    if (expectedIndex == index || targetImage.Source is BitmapImage bi && bi.UriSource != null
                    && bi.UriSource.OriginalString.Contains(expectedPath.Replace('\\', '/')))
                    {
                        visibleImage.Visibility = Visibility.Collapsed;
                        targetImage.Visibility = Visibility.Visible;

                        isFrontVisible = !isFrontVisible;

                        currentImageIndex = expectedIndex;
                    }
                }
                catch { }
            };

            bmp.ImageFailed += (s, e) =>
            {
                targetImage.Source = null;
                targetImage.Visibility = Visibility.Collapsed;
            };

            try
            {
                bmp.UriSource = uri;
                targetImage.Source = bmp;
            }
            catch
            {
                targetImage.Source = null;
                targetImage.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateBreadcrumb(string path)
        {
            breadcrumbPanel.Children.Clear();

            try
            {
                var nodes = new List<DirectoryInfo>();

                string combinedPath = comicsPath;
                foreach (var item in path.Replace(comicsPath, string.Empty).Split(Path.DirectorySeparatorChar))
                {
                    nodes.Add(new DirectoryInfo(Path.Combine(comicsPath, item)));
                }

                foreach (var node in nodes)
                {
                    string display = string.IsNullOrEmpty(node.Name) ? node.FullName : node.Name;
                    if (!string.IsNullOrEmpty(display))
                    {
                        display = char.ToUpper(display[0], CultureInfo.CurrentCulture) + (display.Length > 1 ? display[1..] : string.Empty);
                    }

                    string targetPath = node.FullName;

                    var btn = new Button
                    {
                        Content = display,
                        Tag = targetPath,
                        Margin = new Thickness(0),
                        CornerRadius =new CornerRadius(0),
                    };

                    btn.Click += (s, e) =>
                    {
                        if (btn.Tag is string p && Directory.Exists(p))
                        {
                            LoadFolder(p);
                            comicsImage.Source = null;
                            comicsImageBack.Source = null;
                        }
                    };

                    breadcrumbPanel.Children.Add(btn);

                    /*if (node != nodes.Last())
                    {
                        var sep = new TextBlock
                        {
                            Text = "›",
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(6, 0, 6, 0)
                        };
                        breadcrumbPanel.Children.Add(sep);
                    }*/
                }
            }
            catch
            {
                breadcrumbPanel.Children.Clear();
            }
        }

        private void ComicImage_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            var pt = e.GetCurrentPoint(comicsImage);
            int delta = pt.Properties.MouseWheelDelta;

            if (comicsImages.Count == 0)
            {
                e.Handled = true;
                return;
            }

            int newIndex = currentImageIndex;
            if (delta > 0)
                newIndex = Math.Max(0, currentImageIndex - 1);
            else if (delta < 0)
                newIndex = Math.Min(comicsImages.Count - 1, currentImageIndex + 1);

            if (newIndex != currentImageIndex)
            {
                ShowImageAt(newIndex);
            }

            e.Handled = true;
        }

        private void FolderPathTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            FilterFolderList(folderPathTextBox.Text);
        }

        private void FilterFolderList(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                folderListView.ItemsSource = dirs;
                return;
            }

            var filtered = dirs
                .Where(d => d.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

            folderListView.ItemsSource = filtered;
        }
    }
}