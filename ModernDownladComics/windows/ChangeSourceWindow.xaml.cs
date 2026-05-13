using ComicsInfraLib.Helpers;
using ComicsLib.Models;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownloadComics.Services;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChangeSourceWindow : Window
    {
        private static WindowService WindowService => WindowService.Instance;
        public ObservableCollection<string> Hosts { get; set; } = [];
        public Comic Comic { get; set; }
        public ChangeSourceWindow(Comic comic)
        {
            InitializeComponent();

            AppWindow.Resize(new(200, 115));
            AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

            WindowService.SetOwner(this);
            WindowService.Center(this);

            OverlappedPresenter presenter = OverlappedPresenter.Create();
            presenter.IsModal = true;
            presenter.SetBorderAndTitleBar(true, false);

            AppWindow.SetPresenter(presenter);

            foreach (var host in App.Current.Services.GetRequiredService<ISettingsService>().GetOptions().Hosts)
            {
                Hosts.Add(host);
            }

            Comic = comic;

            AppWindow.Show();
        }

        private async void SelectBTN_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ComicsHostCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comicsHostCMB.SelectedItem is string host)
            {
                var service = App.Current.Services.GetRequiredService<IHtmlParserService>();
                HtmlNode? bodyNode = service?.LoadBody(Comic.HtmlBody ?? "");
                if (bodyNode != null)
                {
                    HtmlNode? node = service?.FindNodeWithAttribute(bodyNode, host, "href");
                    if (node != null)
                    {
                        string newUrl = node.GetAttributeValue("href", "");
                        Comic.URL = newUrl;
                        Comic.Host = RegexUtility.HostRegex().Match(newUrl).Value;
                    }
                }
            }
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            App.Current.Services.GetRequiredService<IStateRepository>().Save();
        }
    }
}
