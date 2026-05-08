using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsLib.Utilities;
using ComicsLib.Utility;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using FuzzierSharp.Extractor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Models;
using ModernDownladComics.Models.View;
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
        ViewModel.SearchDialogEvent += async (jd, pathService, name) =>
        {
            ContentDialog dialog = new()
            {
                Title = "Search",
                Content = new SearchPage(ViewModel.AddToPanel(FileUtility.ComicsDirectory, "Manga"),
              ViewModel.AddToPanel(pathService.BackupDirPath, "Backup"), jd ?? "Download not found",
                $"Do you want to add {name}"),
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

    private void FileCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ViewModel.FileChanged();
    }


    
}
