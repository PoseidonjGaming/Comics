using DownloadComics.models;
using DownloadComics.utilities;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Windows;


namespace DownloadComics.windows
{
    /// <summary>
    /// Logique d'interaction pour ResolveCaptcha.xaml
    /// </summary>
    public partial class ResolveCaptcha : Window
    {
        private readonly string targetUrl;


        private CoreWebView2Environment? env;
        private EventHandler<CoreWebView2WebResourceRequestedEventArgs>? webResourceRequestHandler;
        private CancellationTokenSource? cts;
        private readonly Action<string?> funcHtml;
        private readonly RetrieveSource source;


        private AppState State => AppStateStore.Instance;

        public ResolveCaptcha(string url, RetrieveSource retrieveSource, Action<string?> func)
        {
            InitializeComponent();
            targetUrl = url ?? string.Empty;
            funcHtml = func;
            source = retrieveSource;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
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

                browser.CoreWebView2.NavigationCompleted += async (s, ev) =>
                {
                    if (ev.IsSuccess)
                    {
                        try
                        {
                            

                            if (source == RetrieveSource.HTML)
                            {
                                string js = "document.documentElement.outerHTML";
                                string jsonResult = await browser.CoreWebView2.ExecuteScriptAsync(js);
                                funcHtml.Invoke(JsonSerializer.Deserialize<string>(jsonResult));
                            }
                            else
                            {
                                funcHtml.Invoke(browser.Source.AbsoluteUri);
                            }

                        }
                        catch (Exception)
                        {
                        }
                    }

                    DialogResult = true;
                    Close();
                };

                browser.CoreWebView2.Navigate(targetUrl);
            }
            catch (Exception)
            {
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                try
                {
                    if (env != null)
                    {
                        env = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
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

        public enum RetrieveSource
        {
            URL, HTML
        }
    }
}
