using DownloadComics.utilities;
using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;


namespace DownloadComics.windows
{
    /// <summary>
    /// Logique d'interaction pour ImportWindows.xaml
    /// </summary>
    public partial class ImportWindows : Window
    {
        private CoreWebView2Environment? env;
        private EventHandler<CoreWebView2WebResourceRequestedEventArgs>? webResourceRequestHandler;
        private CancellationTokenSource? cts;

        public ImportWindows(List<string> importedUrls)
        {
            InitializeComponent();
            foreach (string url in importedUrls.OrderBy(url => RegexUtility.HostRegex().Match(url).Value))
            {
                comicsURLsList.Items.Add(url);
            }
        }

        private void ComicsURLsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comicsURLsList.SelectedItem is string selected && !string.IsNullOrWhiteSpace(selected))
            {
                urlTXT.Text = selected;

                if (browser.CoreWebView2 != null)
                {
                    browser.CoreWebView2.Navigate(selected);
                }
                else
                {
                    browser.Source = new Uri(selected);
                }


            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cts = new CancellationTokenSource();

            try
            {
                Task loadRulesTask = AdBlockHelper.LoadRulesAsync();

                var options = new CoreWebView2EnvironmentOptions
                {
                    AdditionalBrowserArguments = "--disable-features=AdsPrivacy"
                };

                string userDataFolder = Path.Combine(Path.GetTempPath(), "WebView2AdBlock");

                env = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options).ConfigureAwait(false);

                await browser.EnsureCoreWebView2Async(env).ConfigureAwait(true);

                await loadRulesTask.ConfigureAwait(false);

                if (webResourceRequestHandler == null && browser.CoreWebView2 != null)
                {
                    browser.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);

                    webResourceRequestHandler = new EventHandler<CoreWebView2WebResourceRequestedEventArgs>(CoreWebView2WebResourceRequested);
                    browser.CoreWebView2.WebResourceRequested += webResourceRequestHandler;
                }
            }
            catch
            {

            }
        }

        private void CoreWebView2WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs args)
        {
            try
            {
                string uri = args.Request.Uri ?? string.Empty;
                if (string.IsNullOrEmpty(uri))
                    return;

                if (!(uri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                      uri.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
                {
                    return;
                }

                if (AdBlockHelper.IsAdOrTracking(uri))
                {
                    var env = browser.CoreWebView2?.Environment;
                    if (env != null)
                    {
                        args.Response = env.CreateWebResourceResponse(null, 200, "OK", "");
                    }
                }
            }
            catch
            {
            }
        }

        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(numberPageTXT.Text, out var pages))
            {
                string js = "document.documentElement.outerHTML";
                string jsonResult = await browser.CoreWebView2.ExecuteScriptAsync(js);

                ComicUtility.MakeComic(urlTXT.Text.Trim(), authorTXT.Text.Trim(), comicNameTXT.Text,
                    pages, this, isScan.IsChecked.GetValueOrDefault(), 
                    JsonSerializer.Deserialize<string>(jsonResult));

                if (Owner is MainWindow)
                {
                    MainWindow.WriteBackup();
                }

                authorTXT.Clear();
                comicNameTXT.Clear();
                numberPageTXT.Clear();
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                try { cts?.Cancel(); } catch { /* ignore */ }

                if (webResourceRequestHandler != null && browser.CoreWebView2 != null)
                {
                    try
                    {
                        browser.CoreWebView2.WebResourceRequested -= webResourceRequestHandler;
                    }
                    catch (Exception)
                    {
                       
                    }

                    webResourceRequestHandler = null;
                }

                try
                {
                    browser.Dispose();
                }
                catch (Exception)
                {
                    
                }

                try
                {
                    if (env != null)
                    {
                        env = null;
                    }
                }
                catch (Exception)
                {
                    
                }
            }
            catch
            {

            }
            finally
            {
                cts?.Dispose();
                cts = null;
            }
        }

        public void Navigate(string url)
        {
            Dispatcher.Invoke(() =>
            {
                urlTXT.Text = url;
                comicsURLsList.SelectedItems.Clear();
                try
                {
                    if (browser.CoreWebView2 == null)
                    {
                        browser.Source = new Uri(url);
                    }
                    else
                    {
                        browser.CoreWebView2.Navigate(url);
                    }
                }
                catch
                {

                }
            });
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            ComicUtility.SearchComic(authorTXT.Text, comicNameTXT.Text);
        }
    }
}
