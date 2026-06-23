using ComicsInfraLib.Models;
using ComicsInfraLib.Models.Views;
using Microsoft.Web.WebView2.Core;
using System.Windows;

namespace DownloadComics.windows
{
    /// <summary>
    /// Logique d'interaction pour BrowserWindow.xaml
    /// </summary>
    public partial class BrowserWindow : Window
    {
        private BrowserPageViewModel ViewModel { get; set; }
        private EventHandler<CoreWebView2WebResourceRequestedEventArgs>? webResourceRequestHandler;
        public BrowserWindow(WebPageArgs args)
        {
            InitializeComponent();
            ViewModel = new();
            ViewModel.Init(args);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await browser.EnsureCoreWebView2Async();

            browser.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            browser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            browser.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
            browser.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
            browser.CoreWebView2.Settings.IsStatusBarEnabled = false;
            browser.CoreWebView2.Profile?.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.Cookies | 
                CoreWebView2BrowsingDataKinds.CacheStorage | CoreWebView2BrowsingDataKinds.AllProfile);

            browser.CoreWebView2.Navigate(ViewModel.URL);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ViewModel.CleanUp();

            if (browser != null)
            {
                try
                {
                    browser.CoreWebView2.WebResourceRequested -= webResourceRequestHandler;
                }
                catch { }
                webResourceRequestHandler = null;
            }
        }

        private async void Browser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            await ViewModel.OnNavigationCompleted(async js => await browser.CoreWebView2.ExecuteScriptAsync(js),
                browser.Source.AbsoluteUri);
            DialogResult = true;
        }
    }
}
