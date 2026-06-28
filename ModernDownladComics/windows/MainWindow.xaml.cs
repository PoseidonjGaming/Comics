using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsLocalizationLib;
using ComicsServiceLib.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Pages;
using ModernDownladComics.Services;
using ModernDownladComics.windows;
using ModernDownloadComics.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly IStateRepository _stateRepository;
        public MainWindow()
        {
            InitializeComponent();
            AppWindow.Resize(new(1750, 900));

            DisplayArea displayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Primary);
            AppWindow.Move(new(displayArea.WorkArea.X + (displayArea.WorkArea.Width - 1750) / 2,
                displayArea.WorkArea.Y + (displayArea.WorkArea.Height - 900) / 2));

            AppWindow.SetIcon("Assets/download comics.ico");
            WindowService.Instance.InitOwner(this);
            var webService = App.Current.Services.GetRequiredService<IWebService>() as WebService;
            webService?.Init(frame);
            
            _stateRepository = App.Current.Services.GetRequiredService<IStateRepository>();
            
            Init();
            var localizationService = App.Current.Services.GetRequiredService<ILocalizationService>();
            var options = App.Current.Services.GetRequiredService<ISettingsService>().GetOptions();
            localizationService.LoadLang(options.Lang);

            frame.Navigate(typeof(MainPage));

            OverlappedPresenter presenter = OverlappedPresenter.Create();
            presenter.IsResizable = false;
            AppWindow.SetPresenter(presenter);
        }

        private async void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "add":
                        frame.Navigate(typeof(AddPage));
                        break;
                    case "archive":
                        frame.Navigate(typeof(ArchivePage));
                        break;
                    case "import":
                        frame.Navigate(typeof(ImportPage));
                        break;
                    case "send":
                        if (_stateRepository.Comics.Count != 0)
                        {
                            frame.Navigate(typeof(SendPage));
                            navView.SelectedItem = null;
                        }

                        break;
                    case "settings":
                        {
                            dimmingOverlay.Visibility = Visibility.Visible;
                            SettingsWindow settingsWindow = new();
                            settingsWindow.Closed += SettingsWindow_Closed;
                            break;
                        }
                    default:
                        frame.Navigate(typeof(MainPage));
                        break;
                }
            }
        }

        private void SettingsWindow_Closed(object sender, WindowEventArgs args)
        {
            dimmingOverlay.Visibility = Visibility.Collapsed;
            navView.SelectedItem = null;
        }

        public void Init()
        {
            var pathService = App.Current.Services.GetRequiredService<IPathService>();
            if (File.Exists(pathService.BackupFilePath))
                _stateRepository.Comics = new(FileUtility.ReadFile<List<Comic>>(pathService.BackupFilePath) ?? []) ;
            if (File.Exists(pathService.TrackFilePath))
                _stateRepository.Tracks =
                    FileUtility.ReadFile<List<Track>>(pathService.TrackFilePath) ?? [];

            FileUtility.CreateFolder(pathService.BackupDirPath);
            FileUtility.CreateFolder(pathService.ComicsDir);
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            _stateRepository.Save();
        }
    }
}
