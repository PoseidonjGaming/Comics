using ComicsInfraLib.Models.Views;
using ComicsLib.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Utilities;
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
            ModelView.SearchDialogEvent += async (dialogArgs) =>
            {
                string manga = ContentPageUtility.Search(dialogArgs.Name, dialogArgs.Author,
                    FileUtility.ComicsDirectory, "Manga");
                string backup = ContentPageUtility.Search(dialogArgs.Name, dialogArgs.Author,
                    dialogArgs.Path, "Backup");
                ContentDialog dialog = new()
                {
                    Title = "Search",
                    Content = new SearchPage(manga, backup,
                  $"From JDownloader {dialogArgs.Jd}" ?? "Download not found",
                    $"Do you want to delete the backup of {dialogArgs.Name}"),
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
    }
}
