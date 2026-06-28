using ComicsInfraLib.Models.Views;
using ComicsJDownloaderApi.Namespace;
using ComicsLib.Models;
using ComicsLocalizationLib;
using CommunityToolkit.Mvvm.ComponentModel;
using DownloadComics.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DownloadComics.windows
{
    /// <summary>
    /// Logique d'interaction pour ArchiveWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class ArchiveWindow : Window
    {
        public ArchivePageViewModel<Window> ViewModel { get; set; }

        [ObservableProperty]
        public partial string SearchBTNContent { get; set; }
        [ObservableProperty]
        public partial string DeleteBTNContent { get; set; }
        public ArchiveWindow()
        {
            InitializeComponent();
            ViewModel = App.Current.ServiceProvider
                .GetRequiredService<ArchivePageViewModel<Window>>();
            DataContext = ViewModel;

            var localizationService = App.Current.ServiceProvider.GetRequiredService<DownloadLocalizationService>();

            DeleteBTNContent = string.Format(localizationService["Buttons.DeleteBTN"],
                localizationService["Entities.Archive"]);
            SearchBTNContent = string.Format(localizationService["Buttons.SearchBTN"],
                localizationService["Entities.Archive"]);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Load();
        }

        private void FolderTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeItem item)
                ViewModel.SelectedItem = item;
        }
    }
}
