using ComicsLib.Models;
using ComicsLocalizationLib;
using DownloadComics.Models;
using DownloadComics.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DownloadComics.windows.Settings
{
    /// <summary>
    /// Logique d'interaction pour SettingsComicsWindow.xaml
    /// </summary>
    public partial class SettingsComicsWindow : Window
    {
        public DownloadSettingsComicsViewModel ViewModel { get; set; }
        public SettingsComicsWindow(Comic? comic)
        {
            InitializeComponent();
            ViewModel = new(comic);
            DataContext = ViewModel;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.EnableState = string.Format("{0}:",
                App.Current.ServiceProvider.GetRequiredService<DownloadLocalizationService>()
                .Get("SettingsComicsPage.Enabled"));
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel.EnableState = string.Format("{0}:", App.Current.ServiceProvider.GetRequiredService<LocalizationService>()
                .Get($"SettingsComicsPage.Disabled"));
        }
    }
}
