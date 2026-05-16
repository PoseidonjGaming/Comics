using ComicsInfraLib.Models;
using ComicsInfraLib.Models.Views;
using ComicsLib.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Utilities;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ImportPage : Page
{
    public ImportPageViewModel ViewModel { get; set; }

    public ImportPage()
    {
        InitializeComponent();
        ViewModel = App.Current.Services.GetRequiredService<ImportPageViewModel>();
        ViewModel.ScanEvent += async () =>
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
            return res == ContentDialogResult.Primary;
        };
        ViewModel.SearchDialogEvent += async (args) =>
        {
            string manga = ContentPageUtility.Search(args.Name, args.Author,
                FileUtility.ComicsDirectory, "Manga");
            string backup = ContentPageUtility.Search(args.Name, args.Author,
              args.Path, "Backup");
            ContentDialog dialog = new()
            {
                Title = "Search",
                Content = new SearchPage(manga, backup,!string.IsNullOrEmpty(args.Jd)?
                $"From JDownloader {args.Jd}" : "Download not found",
                $"Do you want to add {args.Name}"),
                PrimaryButtonText = "Yes",
                DefaultButton = ContentDialogButton.Primary,
                CloseButtonText = "No",
                XamlRoot = this.XamlRoot
            };
            ContentDialogResult dialogRes = await dialog.ShowAsync();
            return dialogRes == ContentDialogResult.Primary;
        };
        ViewModel.PathEvent += comic =>
        {
            Frame.Navigate(typeof(PathPage), new PathPageArgs(comic, typeof(ImportPage)));
        };

        DataContext = ViewModel;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Load();
    }
}
