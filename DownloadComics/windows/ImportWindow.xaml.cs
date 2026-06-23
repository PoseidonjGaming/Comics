using ComicsInfraLib.Models.Views;
using ComicsLocalizationLib;
using CommunityToolkit.Mvvm.ComponentModel;
using DownloadComics.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DownloadComics.windows
{
    /// <summary>
    /// Logique d'interaction pour ImportWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class ImportWindow : Window
    {
        public ImportPageViewModel<Window, DownloadLocalizationService> ViewModel { get; set; }

        [ObservableProperty]
        public partial string AddBTNContent { get; set; }
        [ObservableProperty]
        public partial string SearchBTNContent { get; set; }
        [ObservableProperty]
        public partial string ClearBTNContent { get; set; }
        public ImportWindow()
        {
            InitializeComponent();
            ViewModel = App.Current.ServiceProvider
                .GetRequiredService<ImportPageViewModel<Window,DownloadLocalizationService>>();
            ViewModel.PathEvent += comic =>
            {
                PathWindow pathWindow = new(comic)
                {
                    Owner = this
                };
                pathWindow.ShowDialog();
            };
            DataContext = ViewModel;

            var localisationService = App.Current.ServiceProvider.GetRequiredService<DownloadLocalizationService>();

            AddBTNContent = string.Format(localisationService["Buttons.AddBTN"],
                localisationService["Entities.Comics"]);
            SearchBTNContent = string.Format(localisationService["Buttons.SearchBTN"],
                localisationService["Entities.Comics"]);
            ClearBTNContent = string.Format(localisationService["Buttons.ClearBTN"],
                localisationService["Entities.Comics"]);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Load();
        }
    }
}
