using ComicsInfraLib.Models.Views;
using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsLocalizationLib;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using DownloadComics.Services;
using DownloadComics.windows.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using System.Windows.Controls;



namespace DownloadComics.windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class MainWindow : Window
    {
        public MainPageViewModel ViewModel { get; set; }

        [ObservableProperty]
        public partial string DeleteBTNContent { get; set; }

        private readonly IStateRepository _stateRepository;
        private readonly ISettingsService _settingsService;
        public MainWindow()
        {
            InitializeComponent();

            _stateRepository = App.Current.ServiceProvider.GetRequiredService<IStateRepository>();
            _settingsService = App.Current.ServiceProvider.GetRequiredService<ISettingsService>();


            ViewModel = App.Current.ServiceProvider.GetRequiredService<MainPageViewModel>();
            ViewModel.Init();
            ViewModel.ChangeSourceRequested += comic =>
            {
                ChangeSourceWindow changeSourceWindow = new(comic)
                {
                    Owner = this
                };
                changeSourceWindow.Show();
            };
            DataContext = ViewModel;

            var localizationService = App.Current.ServiceProvider.GetRequiredService<DownloadLocalizationService>();

            DeleteBTNContent = string.Format(localizationService["Buttons.DeleteBTN"],
                localizationService["Entities.Comics"]);

            Init();
        }
        public void Init()
        {
            var pathService = App.Current.ServiceProvider.GetRequiredService<IPathService>();
            if (File.Exists(pathService.BackupFilePath))
                _stateRepository.SetComics(FileUtility.ReadFile<List<Comic>>(pathService.BackupFilePath) ?? []);
            if (File.Exists(pathService.TrackFilePath))
                _stateRepository.Tracks =
                    FileUtility.ReadFile<List<Track>>(pathService.TrackFilePath) ?? [];

            FileUtility.CreateFolder(pathService.BackupDirPath);
            FileUtility.CreateFolder(pathService.ComicsDir);
        }
        private void FilterTXT_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.FilterComics();
        }

        private void Hosts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.FilterComics();
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new()
            {
                Owner = this
            };
            addWindow.Closed += AddWindow_Closed;
            addWindow.Show();
        }

        private void AddWindow_Closed(object? sender, EventArgs e)
        {
            ViewModel.FilterComics();
        }

        private void ImportLinksItem_Click(object sender, RoutedEventArgs e)
        {
            ImportWindow importWindow = new()
            {
                Owner = this,
            };
            importWindow.Show();
        }

        private void OpenSettingsItem_Click(object sender, RoutedEventArgs e)
        {
            AppSettingsWindow appSettings = new(new(_settingsService.GetOptions()))
            {
                Owner = this
            };
            appSettings.ShowDialog();
        }

        private void CredentialsItem_Click(object sender, RoutedEventArgs e)
        {
            ICredentialsService credentialsService = App.Current.ServiceProvider.GetRequiredService<ICredentialsService>();
            CredentialsSettingsWindow settingsWindow = new(new(credentialsService.GetCredentials()))
            {
                Owner = this
            };
            settingsWindow.ShowDialog();
        }

        private void ArchiveItem_Click(object sender, RoutedEventArgs e)
        {
            ArchiveWindow archiveWindow = new()
            {
                Owner = this
            };
            archiveWindow.ShowDialog();
        }

        private void SendItem_Click(object sender, RoutedEventArgs e)
        {
            SendWindow sendWindow = new()
            {
                Owner = this,
            };
            sendWindow.Closed += SendWindow_Closed;
            sendWindow.ShowDialog();
        }

        private void SendWindow_Closed(object? sender, EventArgs e)
        {
            ViewModel.FilterComics();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _stateRepository.Save();
        }
    }
}