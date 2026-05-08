using ComicsLib.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Models.View;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArchivePage : Page
    {
        public ArchivePageViewModel ModelView { get; set; }
        public ArchivePage()
        {
            InitializeComponent();
            ModelView = App.Current.Services.GetRequiredService<ArchivePageViewModel>();
            ModelView.SearchDialogEvent += async (jd, pathService, name) =>
            {
                ContentDialog dialog = new()
                {
                    Title = "Search",
                    Content = new SearchPage(ModelView.AddToPanel(FileUtility.ComicsDirectory, "Manga"),
                  ModelView.AddToPanel(pathService.BackupDirPath, "Backup"),
                  $"From JDownloader {jd}" ?? "Download not found",
                    $"Do you want to delete the backup of {name}"),
                    PrimaryButtonText = "Yes",
                    DefaultButton = ContentDialogButton.Primary,
                    CloseButtonText = "No",
                    XamlRoot = this.XamlRoot
                };
                ContentDialogResult result = await dialog.ShowAsync();
                return result == ContentDialogResult.Primary;
            };

            ModelView.RestoreDialogEvent += async () =>
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
                return res == ContentDialogResult.Primary;
            };
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ModelView.Load();

        }



        private async void SearchBTN_Click(object sender, RoutedEventArgs e)
        {

        }



        private async void RestoreBTN_Click(object sender, RoutedEventArgs e)
        {


        }



        private void DeleteBTN_Click(object sender, RoutedEventArgs e)
        {
        }
    }


}
