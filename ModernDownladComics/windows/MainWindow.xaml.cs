using ComicsLib.Models;
using ComicsLib.Utility;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public Dictionary<string, string> Loc { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            AppWindow.SetIcon("Assets/download comics.ico");
            WindowService.Instance.InitOwner(this);
            var webService = App.Current.Services.GetRequiredService<IWebService>() as WebService;
            webService?.Init(frame);
            Init();
            App.Current.LocalizationService.LanguageChangedEvent += (data) =>
            {
                Loc = App.Current.LocalizationService.GetData("MainWindow");
                Bindings.Update();
            };
            Loc = App.Current.LocalizationService.GetData("MainWindow");

            AppStateStore.Instance.Comics.Add(new()
            {
                PackageName = "Test 1",
                Author = "Author 1",
                Host = "https://k2s.cc/",
                URL = "https://k2s.cc/file/042a3437140e2/Patreon_-_Corrupted_Waifus_-_Black_Widow__AI_GENERATED_.rar?site=svscomics.com"
            });
            AppStateStore.Instance.Comics.Add(new()
            {
                PackageName = "Test 2",
                Author = "Author 1",
                Host = "https://fboom.me/",
                URL = "https://fboom.me/file/0089aa8033a4b/Patreon_-_Corrupted_Waifus_-_Black_Widow__AI_GENERATED_.rar?site=svscomics.com"
            });
            AppStateStore.Instance.Comics.Add(new()
            {
                PackageName = "Test 2",
                Author = "Author 2",
                Host = "https://florenfile.com/",
                URL = "https://florenfile.com/11575xpl3bwu/Patreon_-_Corrupted_Waifus_-_Black_Widow__AI_GENERATED_.rar.html"
            });

            frame.Navigate(typeof(MainPage));
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
                        if (AppStateStore.Instance.Comics.Count > 0)
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
                    case "lang":
                        {

                            App.Current.LocalizationService.LoadLang("en");
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

        public static void Init()
        {
            var pathService = App.Current.Services.GetRequiredService<IPathService>();
            if (File.Exists(pathService.BackupFilePath))
                AppStateStore.Instance.Comics =
                      new(FileUtility.ReadFile<List<Comic>>(pathService.BackupFilePath) ?? []);

            if (File.Exists(pathService.TrackFilePath))
                AppStateStore.Instance.Tracks =
                    FileUtility.ReadFile<List<Track>>(pathService.TrackFilePath) ?? [];

            FileUtility.CreateFolder(pathService.BackupDirPath);
            FileUtility.CreateFolder(pathService.ComicsDir);
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            //App.Current.Services.GetRequiredService<IStateRepository>().Save();
        }
    }
}
