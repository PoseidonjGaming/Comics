using ComicsInfraLib.Models.Views.Settings;
using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using DownloadComics.Models;
using DownloadComics.Services;
using DownloadComics.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace DownloadComics.windows.Settings
{
    /// <summary>
    /// Logique d'interaction pour AppSettingsWindow.xaml
    /// </summary>
    public partial class AppSettingsWindow : Window
    {
        public SettingsAppPageViewModel<Window, DownloadLocalizationService> ViewModel { get; set; }
        public SettingsInputModel InputModel { get; set; }

        private readonly IPickerDialog<Window> _pickerService;
        private readonly ISettingsService _settingsService;

        public AppSettingsWindow(SettingsInputModel inputModel)
        {
            InitializeComponent();

            _pickerService = App.Current.ServiceProvider.GetRequiredService<IPickerDialog<Window>>();
            _settingsService = App.Current.ServiceProvider.GetRequiredService<ISettingsService>();

            InputModel = inputModel;

            ViewModel = App.Current.ServiceProvider
                .GetRequiredService<SettingsAppPageViewModel<Window, DownloadLocalizationService>>();
            ViewModel.Init();
            ViewModel.PathChanged += path => InputModel.Path = path;
            ViewModel.LangChanged += lang => InputModel.Lang = lang;

            DataContext = ViewModel;

        }

        private void Lang_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ViewModel.LoadLang();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var settingsService = App.Current.ServiceProvider.GetRequiredService<ISettingsService>();
            settingsService.SetOptions(SettingsUtility.ToOptions(InputModel));
            settingsService.SaveOptions();

            App.Current.ServiceProvider.GetRequiredService<ICredentialsService>().SaveCredentials();
        }

        private void ComicItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsComicsWindow settingComic = new(InputModel.Comic)
            {
                Owner = this
            };
            settingComic.Show();
        }

        private void HostsItem_Click(object sender, RoutedEventArgs e)
        {
            CollectionSettingsWindow hostsWindow = new(new(InputModel.Hosts, true), "Host", "Scan")
            {
                Owner = this
            };
            hostsWindow.Show();
        }

        private void ConfirmHostsItem_Click(object sender, RoutedEventArgs e)
        {
            CollectionSettingsWindow hostsWindow = new(new(InputModel.Confirms, true), "Host", "Confirm")
            {
                Owner = this
            };
            hostsWindow.Show();
        }

        private void ExcludedHostsItem_Click(object sender, RoutedEventArgs e)
        {
            CollectionSettingsWindow hostsWindow = new(new(InputModel.ExcludedHosts, true), "Host", "Excluded")
            {
                Owner = this
            };
            hostsWindow.Show();
        }

        private void PathsItem_Click(object sender, RoutedEventArgs e)
        {
            CollectionSettingsWindow hostsWindow = new(new(InputModel.Paths, false), "Path", "Paths")
            {
                Owner = this
            };
            hostsWindow.Show();
        }

        private async void ImportItem_Click(object sender, RoutedEventArgs e)
        {
            string path = await _pickerService.FileOpenDialog(this, "FileDialog.Title");
            if (!string.IsNullOrEmpty(path))
                _settingsService.SetOptions(FileUtility.ReadFile<Options>(path) ?? new());
        }

        private async void ExportItem_Click(object sender, RoutedEventArgs e)
        {
            string path = await _pickerService.SaveFileDialog(this, "SaveFileDialog.Title");
            if (!string.IsNullOrEmpty(path))
                FileUtility.WriteFile(path, _settingsService.GetOptions());
        }
    }
}
