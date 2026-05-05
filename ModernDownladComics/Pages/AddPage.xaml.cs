using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsLib.Services;
using ComicsServiceLib;
using FuzzierSharp.Extractor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Models;
using SearchComicsLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPage : Page
    {
        public Comic Comic { get; set; }
        public ObservableCollection<Comic> Comics { get; set; }
        private readonly IComicsBuilderService comicService;
        private readonly JdownloaderService jdownloaderService;

        public AddPage()
        {
            InitializeComponent();

            Comic = new();
            Comics = AppStateStore.Instance.Comics;

            comicService = App.Services.GetRequiredService<IComicsBuilderService>();
            jdownloaderService = App.Services.GetRequiredService<JdownloaderService>();
        }

        private async void AddComicBTN_Click(object sender, RoutedEventArgs e)
        {
            AddComic();
        }

        private async void AddComic()
        {
            ContentDialog dialog = new()
            {
                Title = "Scan the url",
                Content = "Do you want to scan the url",
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No",
                XamlRoot = this.XamlRoot
            };

            var res = await dialog.ShowAsync();
            Comic? comic = await comicService.MakeComics(Comic.BaseURL, Comic.Author.Trim(), Comic.PackageName.Trim(), Comic.NumberPages,
                  res == ContentDialogResult.Primary);
            Frame.Navigate(typeof(PathPage), new PathPageArgs(comic, typeof(AddPage)));

        }

        private async void SearchComicBTN_Click(object sender, RoutedEventArgs e)
        {
            string? jd = await jdownloaderService.GetComicJdownloader(Comic.Author, Comic.PackageName);

            ContentDialog dialog = new()
            {
                Title = "Search",
                Content = new SearchPage(AddToPanel(FileService.ComicsDirectory, "Manga"),
                    AddToPanel(FileService.BackupDirPath, "Backup"), jd ?? "Download not found",
                    $"Do you want to add {Comic.PackageName}"),
                PrimaryButtonText = "Yes",
                DefaultButton = ContentDialogButton.Primary,
                CloseButtonText = "No",
                XamlRoot = this.XamlRoot
            };
            ContentDialogResult dialogRes = await dialog.ShowAsync();
            if (dialogRes == ContentDialogResult.Primary)
            {
                AddComic();
            }
        }

        private string AddToPanel(string path, string from)
        {
            if (Comic == null)
                return "null";
            string authorPath = SearchUtility.GetAuthorPath(Comic.Author, path);

            IEnumerable<ExtractedResult<string>> results = SearchUtility.GetComics(authorPath,
                Comic.PackageName);
            ExtractedResult<string>? res = results.FirstOrDefault();

            if (res == null)
                return "Comic not found";

            string fromPath = res.Value.Replace(authorPath, string.Empty)[1..];

            return $"From {from}: {SearchUtility.CountPage(res.Value)} pages - {fromPath}";
        }

        private void ClearBTN_Click(object sender, RoutedEventArgs e)
        {
            Comic.Reset();
        }

        private void DeleteBTN_Click(object sender, RoutedEventArgs e)
        {
            if (comicLST.SelectedItem is Comic comic)
            {
                AppStateStore.Instance.Comics.Remove(comic);
                
                FileService.WriteFile(FileService.TrackFilePath,
                    AppStateStore.Instance.Tracks);
                FileService.WriteFile(FileService.BackupFilePath,
                    AppStateStore.Instance.Comics);
            }
                
        }
    }
}
