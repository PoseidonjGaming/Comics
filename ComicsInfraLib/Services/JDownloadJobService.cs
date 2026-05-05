using ComicsLib.Models;
using ComicsServiceLib.UI;
using JDownloader;
using JDownloader.Model;

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

            List<OfflineLink> offlineLinks = [];
            JDownloaderClient client = await jdownloaderService.GetClient();
            int addTry = 0;

            do
            {
                addTry++;
                progress = 0;
                jobState.UpdateState("Starting job", true);
                jobState.UpdateTry($"Try {addTry}");
                jobState.UpdateProgess(progress, false);



                await jdownloaderService.Reset();
                listenerService.Count = 0;

                await AddLinks(comic => false, token);

                jobState.UpdateState("Wait to finished job", false);
                jobState.UpdateProgess(progress, false);
                

                offlineLinks = await listenerService.WaitJob();

                offlineLinks.ForEach(async ol =>
                {
                    JdownloaderService.ChangeUrl(State.Comics.First(c => c.UUID == ol.JobUUID),
                        settingsService.GetOptions().Hosts, state =>
                    jobState.UpdateState(state, false));
                });

                progress = 0;
                jobState.UpdateProgess(progress, true);

                await jdownloaderService.RemoveLinks();
            } while (offlineLinks != null && offlineLinks.Count != 0);

            listenerService.Count = 0;
            jobState.ClearState();
            jobState.UpdateTry("Final try");
            jobState.UpdateProgess(State.Comics.Count, true);
            

            await AddLinks(comic =>
            !settingsService.GetOptions().Confirms.Contains(comic.Host), token);

            jobState.UpdateState("Wait to finished job", false);
            offlineLinks = await listenerService.WaitJob();

            jobState.UpdateState("Set links disabled", false);
            await DisableLinks(client);

            jobState.UpdateState("Sort links", false);
            await SortPackages(client);

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
        private async Task DisableLinks(JDownloaderClient client)
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