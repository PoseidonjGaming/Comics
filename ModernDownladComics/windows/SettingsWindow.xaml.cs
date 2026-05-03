using ComicsLib.Models;
using ComicsLib.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Models;
using ModernDownladComics.Pages;
using ModernDownloadComics.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
            AppWindow.Resize(new(1000, 700));
            AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;
            WindowService.SetOwner(this);
            WindowService.Center(this);

            OverlappedPresenter presenter = OverlappedPresenter.Create();
            presenter.IsModal = true;
            presenter.SetBorderAndTitleBar(true, true);
            presenter.IsResizable = false;

            AppWindow.SetPresenter(presenter);

            AppWindow.TitleBar.ExtendsContentIntoTitleBar= true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.ButtonForegroundColor = Colors.White;

            AppWindow.Show();

            options = App.Services.GetRequiredService<ISettingsService>().GetOptions();
            Host = new(options.Hosts);
            Confirm = new(options.Confirms);
            Excluded = new(options.ExcludedHosts);
            Path = new(options.Paths);

            credentials = App.Services.GetRequiredService<ICredentialsService>().GetCredentials();

            settingsFrame.Navigate(typeof(SettingsComicPage), 
                new SettingsPageArgs<Comic?>(options.Comic, false));
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
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
            options.Confirms= [.. Confirm];
            options.ExcludedHosts = [.. Excluded];
            options.Paths = [.. Path];
            App.Services.GetRequiredService<ISettingsService>().SaveOptions();

            App.Services.GetRequiredService<ICredentialsService>().SaveCredentials();
        }
    }
}
