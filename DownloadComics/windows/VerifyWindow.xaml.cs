using DownloadComics.models;
using DownloadComics.resources.verify;
using DownloadComics.services;
using JDownloader;
using JDownloader.Model;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace DownloadComics.windows
{
    /// <summary>
    /// Logique d'interaction pour VerifyWindows.xaml
    /// </summary>
    public partial class VerifyWindow : Window
    {
        private CancellationTokenSource? _verifyTokenSource;
        private readonly ObservableCollection<string> _states = [];
        private readonly ListenerService _listenerService = ListenerService.Instance;
        private readonly JdownloaderService _jdownloaderService = JdownloaderService.Instance;

        private int _try = 0;

        private static AppState State
        {
            get
            {
                return AppStateStore.Instance;
            }
        }

        public VerifyWindow()
        {
            InitializeComponent();
            urlListBox.ItemsSource = _states;
            DataContext = this;
        }

        private void JobToggleBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (_verifyTokenSource == null || _verifyTokenSource.IsCancellationRequested)
            {
                _verifyTokenSource?.Dispose();
                _verifyTokenSource = new CancellationTokenSource();

                progressBar.IsIndeterminate = true;

                jobToggleBtn.IsChecked = true;
                jobToggleBtn.Content = VerifyStrings.Verify_Stop_Button;

                RunVerify();
            }
        }

        private void JobToggleBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_verifyTokenSource != null && !_verifyTokenSource.IsCancellationRequested)
            {
                _verifyTokenSource.Cancel();
                progressBar.IsIndeterminate = false;
                jobToggleBtn.IsChecked = false;
                jobToggleBtn.Content = VerifyStrings.Verify_Start_Button;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _verifyTokenSource = new CancellationTokenSource();
            progressBar.IsIndeterminate = true;
            jobToggleBtn.IsChecked = true;
            jobToggleBtn.Content = VerifyStrings.Verify_Stop_Button;
            _listenerService.StartAsync(() => _jdownloaderService.startingCount.ToString(),
                () => State.GetComics().Count.ToString());
            RunVerify();
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _verifyTokenSource?.Cancel();
            _verifyTokenSource?.Dispose();
            _verifyTokenSource = null;
            _listenerService.Dispose();
        }
        private void RunVerify()
        {
            if (_verifyTokenSource == null) return;

            var ct = _verifyTokenSource.Token;

            Options options = JsonConvert.DeserializeObject<Options>(Properties.Settings.Default.Options) ?? new Options();

            Task.Run(async () =>
               {
                   List<OfflineLink> offlineLinks = [];

                   JDownloaderClient client = await _jdownloaderService.GetInstanceAsync();
                   do
                   {
                       Dispatcher.Invoke(() =>
                       {
                           _states.Clear();
                           _states.Add(VerifyStrings.Verify_Start_Job);
                           _try++;
                           tryLBL.Content = $"{VerifyStrings.Verify_Try_Label} {_try}";
                           progressBar.IsIndeterminate = false;
                       });
                       _jdownloaderService.ClearJobs();

                       await _jdownloaderService.SetCrawledPackageCount();
                       await AddLinks(comic => false, ct);

                       try { await Task.Delay(1000); } catch (OperationCanceledException) { break; }

                       Dispatcher.Invoke(() =>
                       {
                           _states.Add(VerifyStrings.Verify_Wait);
                           progressBar.IsIndeterminate = true;
                       });

                       offlineLinks = await _listenerService.WaitJob();

                       offlineLinks.ForEach(async ol =>
                       {
                           JdownloaderService.ChangeUrl(State.GetComics().First(c => c.UUID == ol.JobUUID), options.Hosts, state =>
                           {
                               Dispatcher.Invoke(() =>
                               {
                                   _states.Add(state);
                               });
                           });
                       });

                       Dispatcher.Invoke(() =>
                       {
                           progressBar.IsIndeterminate = true;
                           progressBar.Value = 0.0;
                       });
                       await _jdownloaderService.RemoveLinks();
                   } while (offlineLinks != null && offlineLinks.Count != 0);

                   Dispatcher.Invoke(() =>
                   {
                       _states.Add(VerifyStrings.Verify_No_More_Offline);
                       progressBar.IsIndeterminate = false;
                       progressBar.Value = progressBar.Maximum;
                   });

                   await AddLinks(comic => !options.Confirms.Contains(comic.Host), ct);

                   Dispatcher.Invoke(() => _states.Add(VerifyStrings.Verify_Wait));
                   offlineLinks = await _listenerService.TaskCompletionSource.Task;

                   Dispatcher.Invoke(() => _states.Add(VerifyStrings.Verify_Disable));
                   await DisableLinks(client);

                   Dispatcher.Invoke(() => _states.Add(VerifyStrings.Verify_Sorting));
                   await SortPackages(client);

                   Dispatcher.Invoke(() => _states.Add(VerifyStrings.Verify_Set_Name_Comment));
                   await FinishedLink(client);

                   State.ClearComics();
                   Dispatcher.Invoke(() =>
                   {
                       jobToggleBtn.IsChecked = false;
                       DialogResult = true;
                   });
               }, ct);
        }

        private async Task DisableLinks(JDownloaderClient client)
        {
            foreach (Comic comic in State.GetComics().Where(comic => !comic.Enabled))
            {
                List<CrawledLink> cralwedLinks = await _jdownloaderService.GetCrawledLink(comic.UUID);
                await client.LinkGrabberV2.SetEnabled([.. cralwedLinks.Select(l => l.Uuid)], [], false);
            }
        }

        private async Task FinishedLink(JDownloaderClient client)
        {
            foreach (Comic comic in State.GetComics())
            {
                List<CrawledLink> crawledLinks = await _jdownloaderService.GetCrawledLink(comic.UUID);
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

        private async Task AddLinks(Func<Comic, bool> autoStart, CancellationToken ct)
        {
            _jdownloaderService.ClearJobs();
            Dispatcher.Invoke(() =>
            {
                progressBar.Value = 0;
                progressBar.Minimum = 0;
                progressBar.Maximum = State.GetComics().Count;
            });
            foreach (Comic comic in State.GetComics())
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                await _jdownloaderService.AddLinks(comic, autoStart.Invoke(comic), state =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        _states.Add(state);
                        progressBar.Value++;
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
    }
}