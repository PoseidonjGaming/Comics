using ComicsLib.Models;
using ComicsLib.Services;
using ComicsLib.Utilities;
using FuzzierSharp.Extractor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Models;
using SearchComicsLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ImportPage : Page
{
    public ObservableCollection<string> URLS { get; set; } = [];
    public ObservableCollection<string> Files { get; set; }
    public Comic Comic { get; set; }

    private readonly ComicService comicService;
    private readonly JdownloaderService jdownloaderService;

    public ImportPage()
    {
        InitializeComponent();
        Comic = new Comic();

        comicService = new(comic => Frame.Navigate(typeof(PathPage),
            new PathPageArgs(comic, typeof(ImportPage))),
                (s, ret, source) => Frame.Navigate(typeof(BrowserPage),
                new WebPageArgs(s, ret, source)));

        Files = new ObservableCollection<string>(Directory.GetFiles(FileService.ComicsDir)
           .OrderBy(File.GetLastWriteTimeUtc).Select(dir => Path.GetFileName(dir)));

        jdownloaderService = App.Services.GetRequiredService<JdownloaderService>();
    }

    private async void AddBTN_Click(object sender, RoutedEventArgs e)
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
        if (res == ContentDialogResult.Primary)
        {
            comicService.MakeComic(Comic.BaseURL, Comic.Author, Comic.PackageName, Comic.NumberPages,
           res == ContentDialogResult.Primary, App.Services?.GetRequiredService<ISettingsService>());

            Comic.Reset();
        }
    }

    private void ClearBTN_Click(object sender, RoutedEventArgs e)
    {
        Comic.Reset();
    }

    private void FileCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox box && box.SelectedValue is string file)
        {
            string path = Path.Combine(FileService.ComicsDir, file);
            URLS = new ObservableCollection<string>(JsonUtility.GetURLS(File.ReadAllText(path)));
        }
    }

    private async void searchBTN_Click(object sender, RoutedEventArgs e)
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

    private void AddComic()
    {
        
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
}
