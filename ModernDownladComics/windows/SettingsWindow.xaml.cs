using ComicsInfraLib.Models;
using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.Storage.Pickers;
using ModernDownladComics.Models;
using ModernDownladComics.Pages;
using ModernDownladComics.Utilities;
using ModernDownloadComics.Services;
using System;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsWindow : Window
    {
        private static WindowService WindowService => WindowService.Instance;
        private readonly Options options;
        private readonly JDCredentials credentials;
        private readonly SettingsInputModel settingsInputModel;
        private readonly IPickerDialog<WindowId> pickerDialogService;

        public SettingsWindow()
        {
            InitializeComponent();
            AppWindow.Resize(new(1150, 700));
            AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;
            WindowService.SetOwner(this);
            WindowService.Center(this);

            OverlappedPresenter presenter = OverlappedPresenter.Create();
            presenter.IsModal = true;
            presenter.SetBorderAndTitleBar(true, true);
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;

            AppWindow.SetPresenter(presenter);

            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.ButtonForegroundColor = Colors.White;

            options = App.Current.Services.GetRequiredService<ISettingsService>().GetOptions();
            settingsInputModel = new(options);

            credentials = App.Current.Services.GetRequiredService<ICredentialsService>().GetCredentials();

            pickerDialogService = App.Current.Services.GetRequiredService<IPickerDialog<WindowId>>();

            AppWindow.Show();

            settingsFrame.Navigate(typeof(SettingsAppPage),
                new SettingsPageArgs<SettingsInputModel>(settingsInputModel));
        }

        public async void NavigationView_SelectionChangedAsync(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "comic":
                        {
                            settingsFrame.Navigate(typeof(SettingsComicPage),
                                new SettingsPageArgs<Comic?>(settingsInputModel.Comic));
                            break;
                        }
                    case "search":
                        {
                            SettingsCollectionPageArg<WindowId> param =
                                new(settingsInputModel.Hosts, AppWindow.Id, true, "Host");
                            settingsFrame.Navigate(typeof(SettingsHostsPage), param);
                            break;
                        }
                    case "confirm":
                        {
                            SettingsCollectionPageArg<WindowId> param = new(settingsInputModel.Confirms, AppWindow.Id,
                                true, "Host");
                            settingsFrame.Navigate(typeof(SettingsHostsPage), param);
                            break;
                        }
                    case "excluded":
                        {
                            SettingsCollectionPageArg<WindowId> param = new(settingsInputModel.ExcludedHosts, AppWindow.Id,
                                true, "Host");
                            settingsFrame.Navigate(typeof(SettingsHostsPage), param);
                            break;
                        }
                    case "path":
                        {
                            SettingsCollectionPageArg<WindowId> param = new(settingsInputModel.Paths, AppWindow.Id,
                                false, "Path");
                            settingsFrame.Navigate(typeof(SettingsHostsPage), param);
                            break;
                        }
                    case "credentials":
                        {
                            settingsFrame.Navigate(typeof(SettingsCredentials),
                                new SettingsPageArgs<JDCredentials>(credentials));
                            break;
                        }
                    case "import_settings":
                        {
                            string path = await pickerDialogService.FileOpenDialog(AppWindow.Id,
                                "FileDialog.Title");
                            if (string.IsNullOrEmpty(path))
                                break;
                            App.Current.Services.GetRequiredService<ISettingsService>()
                                .SetOptions(FileUtility.ReadFile<Options>(path) ?? new());
                            AppNotification notification = new AppNotificationBuilder()
                                .AddText("Settings import successful")
                                .BuildNotification();
                            AppNotificationManager.Default.Show(notification);
                            navView.SelectedItem = null;
                            break;
                        }
                    case "export_settings":
                        {
                            string path = await pickerDialogService.SaveFileDialog(AppWindow.Id,
                                "SaveFileDialog.Title");
                            if (string.IsNullOrEmpty(path))
                                break;
                            FileUtility.WriteFile(Path.Combine(path, "settings.json"), options);
                            AppNotification notification = new AppNotificationBuilder()
                                .AddText("Settings export successful")
                                .BuildNotification();
                            AppNotificationManager.Default.Show(notification);
                            navView.SelectedItem = null;
                            break;
                        }
                    default:
                        {
                            settingsFrame.Navigate(typeof(SettingsAppPage),
                                new SettingsPageArgs<SettingsInputModel>(settingsInputModel));
                            break;
                        }
                }
            }
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            var settingsService = App.Current.Services.GetRequiredService<ISettingsService>();
            settingsService.SetOptions(SettingsUtility.ToOptions(settingsInputModel));
            settingsService.SaveOptions();

            App.Current.Services.GetRequiredService<ICredentialsService>().SaveCredentials();
        }
    }
}
