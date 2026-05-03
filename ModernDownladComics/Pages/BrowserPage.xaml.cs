

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.



using ComicsLib.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using ModernDownladComics.Models;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserPage : Page
    {
        private string? targetUrl;


        private TypedEventHandler<CoreWebView2, CoreWebView2WebResourceRequestedEventArgs>? webResourceRequestHandler;
        private CancellationTokenSource? cts;
        private RetrieveSource? source;

        private TaskCompletionSource<string>? _taskSource;

        public BrowserPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is WebPageArgs pageArgs)
            {
                targetUrl = pageArgs.Url;
                source = pageArgs.RetrieveSource;
                _taskSource = pageArgs.Source;
            }
        }

        private async void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                await browser.EnsureCoreWebView2Async();



                browser.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
                browser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                browser.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
                browser.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
                browser.CoreWebView2.Settings.IsStatusBarEnabled = false;

                browser.CoreWebView2.Profile?.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.Cookies | CoreWebView2BrowsingDataKinds.CacheStorage | CoreWebView2BrowsingDataKinds.AllProfile);

                browser.CoreWebView2.Navigate(targetUrl);
            }
            catch (Exception)
            {
            }
        }

        private async void Browser_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                try
                {
                    if (_taskSource != null)
                    {
                        if (source == RetrieveSource.HTML)
                        {
                            string js = "document.documentElement.outerHTML";
                            string jsonResult = await browser.CoreWebView2.ExecuteScriptAsync(js);
                            _taskSource.TrySetResult(JsonSerializer.Deserialize<string>(jsonResult) ?? "empty");
                        }
                        else
                        {
                            _taskSource.TrySetResult(browser.Source.AbsoluteUri);
                        }


                    }



                }
                catch (Exception)
                {
                }
            }
        }

        private void Page_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {

            cts?.Cancel();
            cts?.Dispose();
            cts = null;

            if (browser != null)
            {
                try
                {
                    if (browser.CoreWebView2 != null)
                        browser.CoreWebView2.WebResourceRequested -= webResourceRequestHandler;
                }
                catch { /* ignore */ }

                webResourceRequestHandler = null;

            }
        }


    }
}
