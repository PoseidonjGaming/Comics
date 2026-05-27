using ComicsJDownloaderApi;
using ComicsLib.Models;
using ComicsServiceLib.UI;
using JDownloader;
using JDownloader.Model;
using Newtonsoft.Json.Linq;

namespace ComicsInfraLib.Services
{
    public class JDownloadJobService(JdownloaderService jdownloaderService,
        ListenerService listenerService, ISettingsService settingsService,
        IJobState jobState)
    {
        private int progress;
        private static AppState State => AppStateStore.Instance;

        public async Task RunAsync(CancellationToken token)
        {
            listenerService.StartAsync();

            await RetryLoop(token);

            try
            {
                await Task.Delay(1000, token);
            }
            catch
            {

            }
            await AddLinks(token);

            try
            {
                await Task.Delay(1000, token);
            }
            catch
            {

            }

            await FinalizeLinks(token);

        }

        private async Task RetryLoop(CancellationToken token)
        {
            List<OfflineLink> offlineLinks;
            int tryCount = 0;

            do
            {
                tryCount++;
                PrepareTry(tryCount);
                await ResetTry();

                await TryAddLinks(token);
                offlineLinks = await listenerService.WaitJob();
                await FixLinks(offlineLinks);

                await jdownloaderService.RemoveLinks();
            } while (offlineLinks != null && offlineLinks.Count != 0);
        }

        private void PrepareTry(int tryCount)
        {
            progress = 0;
            jobState.UpdateState("Starting job", true);
            jobState.UpdateTry($"Try {tryCount}");
            jobState.UpdateProgess(progress, false);
            listenerService.SetTask(new());
        }

        private async Task ResetTry()
        {
            await jdownloaderService.Reset();
            listenerService.Count = 0;
        }

        private async Task TryAddLinks(CancellationToken token)
        {
            await AddLinks(comic => false, token);

            jobState.UpdateState("Wait to finished job", false);
            jobState.UpdateProgess(progress, false);
        }

        private async Task FixLinks(List<OfflineLink> offlineLinks)
        {
            offlineLinks.ForEach(async ol =>
            {
                jdownloaderService.ChangeUrl(State.Comics.First(c => c.UUID == ol.JobUUID),
                    settingsService.GetOptions().Hosts, state =>
                jobState.UpdateState(state, false));
            });

            progress = 0;
            jobState.UpdateProgess(progress, true);
        }

        private async Task AddLinks(CancellationToken token)
        {
            listenerService.Count = 0;
            jobState.ClearState();
            jobState.UpdateTry("Final try");
            jobState.UpdateProgess(State.Comics.Count, true);


            await AddLinks(comic =>
            !settingsService.GetOptions().Confirms.Contains(comic.Host), token);

            jobState.UpdateState("Wait to finished job", false);
            await listenerService.WaitJob();
        }

        private async Task FinalizeLinks(CancellationToken token)
        {
            ComicsJDownloaderClient client = await jdownloaderService.GetClient();
            jobState.UpdateState("Set links disabled", false);
            await DisableLinks(client);

            try
            {
                await Task.Delay(1000, token);
            }
            catch
            {

            }

            jobState.UpdateState("Sort links", false);
            await SortPackages(client);

            try
            {
                await Task.Delay(1000, token);
            }
            catch
            {

            }

            jobState.UpdateState("Set name and comment", false);
            await FinishedLink(client);

            jobState.ClearState();
        }

        private async Task AddLinks(Func<Comic, bool> autoStart, CancellationToken ct)
        {
            if (jdownloaderService != null)
            {
                foreach (Comic comic in State.Comics)
                {
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }


                    await jdownloaderService.AddLinks(comic, autoStart.Invoke(comic), state =>
                    {
                        progress++;
                        jobState.UpdateState(state, false);
                        jobState.UpdateProgess(progress, true);
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
        private async Task DisableLinks(ComicsJDownloaderClient client)
        {
            if (jdownloaderService != null)
            {
                foreach (Comic comic in State.Comics.Where(comic => !comic.Enabled))
                {
                    List<CrawledLink> cralwedLinks = await jdownloaderService.GetCrawledLink(comic.UUID);
                    await client.LinkGrabberV2.SetEnabled([.. cralwedLinks.Select(l => l.Uuid)], [], false);
                }
            }
        }
        private static async Task SortPackages(ComicsJDownloaderClient client)
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

        private async Task FinishedLink(ComicsJDownloaderClient client)
        {
            if (jdownloaderService != null)
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
}