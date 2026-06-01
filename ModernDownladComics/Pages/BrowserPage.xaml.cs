

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.



using ComicsInfraLib.Models;
using ComicsInfraLib.Models.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using Windows.Foundation;

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserPage : Page
    {
        public BrowserPageViewModel ViewModel { get; }
        private TypedEventHandler<CoreWebView2, CoreWebView2WebResourceRequestedEventArgs>? webResourceRequestHandler;

        public BrowserPage()
        {
            InitializeComponent();
            ViewModel = new();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is WebPageArgs pageArgs)
            {
                ViewModel.Init(pageArgs);
            }
        }

        private async void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            await browser.EnsureCoreWebView2Async();



            browser.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            browser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            browser.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
            browser.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
            browser.CoreWebView2.Settings.IsStatusBarEnabled = false;

            browser.CoreWebView2.Profile?.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.Cookies | CoreWebView2BrowsingDataKinds.CacheStorage | CoreWebView2BrowsingDataKinds.AllProfile);


            browser.CoreWebView2.Navigate(ViewModel.URL);
        }

        private async void Browser_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            await ViewModel.OnNavigationCompleted(
                         async js => await browser.CoreWebView2.ExecuteScriptAsync(js),
                         browser.Source.AbsoluteUri);
        }

        private void Page_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.CleanUp();
            if (browser != null)
            {
                try
                {
                    browser.CoreWebView2?.WebResourceRequested -= webResourceRequestHandler;
                }
                catch { /* ignore */ }

                webResourceRequestHandler = null;

            }
        }

        private void browser_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
           
        }
    }
}
