using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Pages;
using ModernDownladComics.Services;
using ModernDownladComics.windows;
using ModernDownloadComics.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            WindowService.Instance.InitOwner(this);
            var webService = App.Services?.GetRequiredService<IWebService>() as WebService;
            webService?.Init(frame);
            Init();
            frame.Navigate(typeof(MainPage));
        }

        private async void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                dimmingOverlay.Visibility = Visibility.Visible;
                SettingsWindow settingsWindow = new();
                settingsWindow.Closed += SettingsWindow_Closed;

            }
            else if (args.SelectedItem is NavigationViewItem item)
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
                        if (AppStateStore.Instance.Comics.Count > 0)
                        {
                            frame.Navigate(typeof(SendPage));
                            navView.SelectedItem = null;
                        }
                            
                        break;
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

        public static void Init()
        {
            var pathService = App.Services.GetRequiredService<IPathService>();
            if (File.Exists(pathService.BackupFilePath))
              AppStateStore.Instance.Comics = 
                    new(FileUtility.ReadFile<List<Comic>>(pathService.BackupFilePath) ?? []);

            if (File.Exists(pathService.TrackFilePath))
                AppStateStore.Instance.Tracks = 
                    FileUtility.ReadFile<List<Track>>(pathService.TrackFilePath) ?? [];

            FileUtility.CreateFolder(pathService.BackupDirPath);
            FileUtility.CreateFolder(pathService.ComicsDir);
        }
    }
}
