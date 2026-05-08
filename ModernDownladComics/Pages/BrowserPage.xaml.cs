

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.



using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using ModernDownladComics.Models;
using ModernDownladComics.Models.View;
using System;
using Windows.Foundation;

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserPage : Page
    {
        public BrowserPageViewModel ModelView { get; }
        private TypedEventHandler<CoreWebView2, CoreWebView2WebResourceRequestedEventArgs>? webResourceRequestHandler;

        public BrowserPage()
        {
            InitializeComponent();
            ModelView = new();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is WebPageArgs pageArgs)
            {
                ModelView.Init(pageArgs);
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

            browser.CoreWebView2.Navigate(ModelView.URL);
        }

        private async void Browser_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                await ModelView.OnNavigationCompleted(
                        async js => await browser.CoreWebView2.ExecuteScriptAsync(js),
                        browser.Source.AbsoluteUri);
            }
        }

        private void Page_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {

            ModelView.Cleanup();

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


    }
}
