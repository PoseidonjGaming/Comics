using ComicsInfraLib.Helpers;
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
using ModernDownloadComics.Services;
using System;
using System.Collections.ObjectModel;

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

        private readonly ObservableCollection<string> Host;
        private readonly ObservableCollection<string> Confirm;
        private readonly ObservableCollection<string> Excluded;
        private readonly ObservableCollection<string> Path;


        public SettingsWindow()
        {
            InitializeComponent();
            AppWindow.Resize(new(1050, 700));
            AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;
            WindowService.SetOwner(this);
            WindowService.Center(this);

            OverlappedPresenter presenter = OverlappedPresenter.Create();
            presenter.IsModal = true;
            presenter.SetBorderAndTitleBar(true, true);
            presenter.IsResizable = false;

            AppWindow.SetPresenter(presenter);

            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.ButtonForegroundColor = Colors.White;

            AppWindow.Show();

            options = App.Current.Services.GetRequiredService<ISettingsService>().GetOptions();
            Host = new(options.Hosts);
            Confirm = new(options.Confirms);
            Excluded = new(options.ExcludedHosts);
            Path = new(options.Paths);

            credentials = App.Current.Services.GetRequiredService<ICredentialsService>().GetCredentials();

            settingsFrame.Navigate(typeof(SettingsComicPage),
                new SettingsPageArgs<Comic?>(options.Comic, false));
        }

        public async void NavigationView_SelectionChangedAsync(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "search":
                        settingsFrame.Navigate(typeof(SettingsHostsPage),
                            new SettingsPageArgs<ObservableCollection<string>>(Host, true));
                        break;
                    case "confirm":
                        settingsFrame.Navigate(typeof(SettingsHostsPage),
                            new SettingsPageArgs<ObservableCollection<string>>(Confirm, true));
                        break;
                    case "excluded":
                        settingsFrame.Navigate(typeof(SettingsHostsPage),
                            new SettingsPageArgs<ObservableCollection<string>>(Excluded, true));
                        break;
                    case "path":
                        settingsFrame.Navigate(typeof(SettingsHostsPage),
                            new SettingsPageArgs<ObservableCollection<string>>(Path, false));
                        break;
                    case "credentials":
                        settingsFrame.Navigate(typeof(SettingsCredentials),
                            new SettingsPageArgs<JDCredentials>(credentials, false));
                        break;
                    case "import_settings":
                        {
                            FileOpenPicker dialog = new(AppWindow.Id)
                            {
                                Title ="Import Settings",
                                CommitButtonText = "Import Settigns",
                                SuggestedFolder = @"E:\Manga Scan",

                            };
                            dialog.FileTypeChoices.Add("JSON", [".json"]);

                            var file = await dialog.PickSingleFileAsync();
                            App.Current.Services.GetRequiredService<ISettingsService>()
                                .SetOptions(FileUtility.ReadFile<Options>(file.Path) ?? new());
                            AppNotification notification = new AppNotificationBuilder()
                                .AddText("Settings import successful")
                                .BuildNotification();
                            AppNotificationManager.Default.Show(notification);
                            navView.SelectedItem = null;
                            break;
                        }
                    case "export_settings":
                        {
                            FileSavePicker saveDialog = new(AppWindow.Id)
                            {
                                Title = "Export Settings",
                                CommitButtonText = "Save file",
                                SuggestedFileName = "Settings Download Comics",
                                DefaultFileExtension =".json"
                            };
                            saveDialog.FileTypeChoices.Add("JSON Files", [".json"]);
                            var result = await saveDialog.PickSaveFileAsync();

                            if(result!= null)
                            {
                                Options options = App.Current.Services
                                    .GetRequiredService<ISettingsService>()
                                    .GetOptions();
                                FileUtility.WriteFile(result.Path, options);
                            }
                            break;
                        }
                    default:
                        settingsFrame.Navigate(typeof(SettingsComicPage),
                            new SettingsPageArgs<Comic?>(options.Comic, false));
                        break;
                }
            }
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            options.Hosts = [.. Host];
            options.Confirms = [.. Confirm];
            options.ExcludedHosts = [.. Excluded];
            options.Paths = [.. Path];
            Comic? comic = options.Comic;
            comic?.Host = RegexUtility.HostRegex().Match(comic.URL).Value;
            App.Current.Services.GetRequiredService<ISettingsService>().SaveOptions();

            App.Current.Services.GetRequiredService<ICredentialsService>().SaveCredentials();
        }
    }
}
