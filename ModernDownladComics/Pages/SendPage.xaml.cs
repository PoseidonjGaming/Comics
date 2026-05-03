using ComicsLib.Models;
using ComicsLib.Services;
using JDownloader;
using JDownloader.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SendPage : Page
    {
        public ObservableCollection<string> States { get; set; } = [];

        private CancellationTokenSource? _verifyTokenSource;

        private readonly JdownloaderService jdownloaderService;
        private readonly ListenerService listenerService;
        private readonly ISettingsService settingsService;

        private static AppState State => AppStateStore.Instance;

        public SendPage()
        {
            InitializeComponent();
            jobToggleBTN.Content = "Start";
            tryTXT.Text = "Job not started";

            jdownloaderService = App.Services.GetRequiredService<JdownloaderService>();
            listenerService = App.Services.GetRequiredService<ListenerService>();
            settingsService = App.Services.GetRequiredService<ISettingsService>();
        }

        private void jobToggleBTN_Checked(object sender, RoutedEventArgs e)
        {
            if (_verifyTokenSource == null || _verifyTokenSource.IsCancellationRequested)
            {
                _verifyTokenSource?.Dispose();
                _verifyTokenSource = new CancellationTokenSource();

                progressbar.IsIndeterminate = true;

                jobToggleBTN.IsChecked = true;
                jobToggleBTN.Content = "Stop";

                RunVerify();
            }
        }

        private void jobToggleBTN_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _verifyTokenSource = new CancellationTokenSource();
            progressbar.IsIndeterminate = true;
            jobToggleBTN.IsChecked = true;
            jobToggleBTN.Content = "Stop";
            _ = listenerService.StartAsync();
            RunVerify();

        }

        private void RunVerify()
        {
            if (_verifyTokenSource == null) return;

            var ct = _verifyTokenSource.Token;

            Task.Run(async () =>
            {
                List<OfflineLink> offlineLinks = [];

                JDownloaderClient client = await jdownloaderService.GetClient();
                int addTry = 0;
                do
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        States.Clear();
                        States.Add("Starting job");
                        addTry++;
                        tryTXT.Text = $"Try {addTry}";
                        progressbar.IsIndeterminate = false;
                    });

                    await jdownloaderService.Reset();
                    listenerService.Count = 0;

                    await AddLinks(comic => false, ct);

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        States.Add("Wait to finished job");
                        progressbar.IsIndeterminate = true;
                    });

                    offlineLinks = await listenerService.WaitJob();

                    offlineLinks.ForEach(async ol =>
                    {
                        JdownloaderService.ChangeUrl(State.Comics.First(c => c.UUID == ol.JobUUID),
                            settingsService.GetOptions().Hosts, state =>
                        DispatcherQueue.TryEnqueue(() => States.Add(state)));
                    });

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        progressbar.IsIndeterminate = true;
                        progressbar.Value = 0;
                    });

                    await jdownloaderService.RemoveLinks();
                } while (offlineLinks != null && offlineLinks.Count != 0);

                listenerService.Count = 0;
                DispatcherQueue.TryEnqueue(() =>
                {
                    States.Clear();
                    tryTXT.Text = "Final try";
                    progressbar.IsIndeterminate = false;
                    progressbar.Value = State.Comics.Count;
                });

                await AddLinks(comic =>
                !settingsService.GetOptions().Confirms.Contains(comic.Host), ct);

                DispatcherQueue.TryEnqueue(() => States.Add("Wait to finished job"));
                offlineLinks = await listenerService.WaitJob();

                DispatcherQueue.TryEnqueue(() => States.Add("Set links disabled"));
                await DisableLinks(client);

                DispatcherQueue.TryEnqueue(() => States.Add("Sort links"));
                await SortPackages(client);

                DispatcherQueue.TryEnqueue(() => States.Add("Set name and comment"));
                await FinishedLink(client);


                DispatcherQueue.TryEnqueue(() =>
                {
                    State.Comics.Clear();
                    jobToggleBTN.IsChecked = false;
                    Frame.Navigate(typeof(MainPage));
                });
            }, ct);
        }

        private async Task AddLinks(Func<Comic, bool> autoStart, CancellationToken ct)
        {
            foreach (Comic comic in State.Comics)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }



                await jdownloaderService.AddLinks(comic, autoStart.Invoke(comic), state =>
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        States.Add(state);
                        progressbar.Value++;
                    });
                });

                try
                {
                    await Task.Delay(1000, ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task DisableLinks(JDownloaderClient client)
        {
            foreach (Comic comic in State.Comics.Where(comic => !comic.Enabled))
            {
                List<CrawledLink> cralwedLinks = await jdownloaderService.GetCrawledLink(comic.UUID);
                await client.LinkGrabberV2.SetEnabled([.. cralwedLinks.Select(l => l.Uuid)], [], false);
            }
        }

        private static async Task SortPackages(JDownloaderClient client)
        {
            List<CrawledPackage> packages = await client.LinkGrabberV2.QueryPackages(new()
            {
                BytesTotal = true,
                MaxResults = -1,
                StartAt = 0
            });

            List<CrawledPackage> sortedPackages = [.. packages.OrderBy(p => p.BytesTotal)];
            long[] sortedIds = [.. sortedPackages.Select(p => p.Uuid)];

            for (int i = 0; i < sortedPackages.Count; i++)
            {
                long[] current = [sortedIds[i]];
                long targetId = (i == 0) ? 0L : sortedIds[i - 1];

                await client.LinkGrabberV2.MovePackages(current, targetId);
            }
        }

        private async Task FinishedLink(JDownloaderClient client)
        {
            foreach (Comic comic in State.Comics)
            {
                List<CrawledLink> crawledLinks = await jdownloaderService.GetCrawledLink(comic.UUID);
                if (crawledLinks.Count == 1 && !string.IsNullOrEmpty(comic.GetFilename()))
                {
                    CrawledLink crawledLink = crawledLinks.First();

                    comic.Extansion = Path.GetExtension(crawledLink.Name);

                    await client.LinkGrabberV2.RenameLink(crawledLink.Uuid, comic.GetFilename());
                    await client.LinkGrabberV2.SetComment([crawledLink.Uuid], [],
                        true, $"{comic.NumberPages} pages");
                }
            }
        }


    }
}
