using ComicsJDownloaderApi;
using ComicsLib.Models;
using ComicsLocalizationLib;
using ComicsServiceLib.UI;
using JDownloader.Model;
using System.Net;

namespace ComicsInfraLib.Services
{
    public class JDownloadJobService(JdownloaderService jdownloaderService,
        ISettingsService settingsService, IJobState jobState, 
        IStateRepository stateRepository, LocalizationService localizationService)
    {
        private int progress;

        public async Task RunAsync(CancellationToken token)
        {
            Options options = settingsService.GetOptions();

            HttpListener listener = new();
            listener.Prefixes.Add("http://localhost:12345/");
            listener.Start();

            List<CrawledLink> links = [];
            int tr = 0;
            do
            {
                tr++;
                jobState.UpdateTry($"{localizationService["SendPage.Try"]} {tr}");
                links.Clear();
                await AddLinks(c => false, listener, token);

                token.ThrowIfCancellationRequested();

                stateRepository.Comics.ForEach(async c =>
                {
                    List<CrawledLink> crawledLinks = await jdownloaderService.GetCrawledLink(c.UUID);

                    if (!options.ExcludedHosts.Contains(c.Host) && crawledLinks.Count == 1 &&
                    crawledLinks.All(cl => cl.AvailableLinkState == AvailableLinkState.OFFLINE))
                    {
                        jdownloaderService.ChangeUrl(c, options.Hosts, state =>
                        jobState.UpdateState(state, false));
                    }
                    await Task.Delay(1000, token);
                });

                progress = 0;
                jobState.UpdateProgess(progress, true);

                links = await jdownloaderService.GetCrawledLink();
                
                await Task.Delay(1000, token);
                
                await jdownloaderService.RemoveLinks();
            } while (links.Any(cl => cl.AvailableLinkState == AvailableLinkState.OFFLINE));

            jobState.UpdateTry(localizationService["SendPage.Final_Try"]);
            jobState.ClearState();
            await AddLinks(c => !options.Confirms.Contains(c.Host), listener, token);

            await FinalizeLinks(token);
            listener.Stop();
        }

        private async Task FinalizeLinks(CancellationToken token)
        {
            ComicsJDownloaderClient client = await jdownloaderService.GetClient();
            jobState.UpdateState(localizationService["SendPage.DisableLinks"], false);
            await DisableLinks(client);

            await Task.Delay(1000, token);

            jobState.UpdateState(localizationService["SendPage.SortLinks"], false);
            await SortPackages(client);

            await Task.Delay(1000, token);

            jobState.UpdateState(localizationService["SendPage.SetNameAndComment"], false);
            await FinishedLink(client);

            jobState.ClearState();
        }

        private async Task DisableLinks(ComicsJDownloaderClient client)
        {
            foreach (Comic comic in stateRepository.Comics.Where(comic => !comic.Enabled))
            {
                List<CrawledLink> cralwedLinks = await jdownloaderService.GetCrawledLink(comic.UUID);
                await client.LinkGrabberV2.SetEnabled([.. cralwedLinks.Select(l => l.Uuid)], [], false);
            }
        }

        private async Task FinishedLink(ComicsJDownloaderClient client)
        {
            foreach (Comic comic in stateRepository.Comics)
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

        private static void HandleRequest(HttpListenerContext context, Comic comic)
        {
            switch (context.Request.Url?.AbsolutePath)
            {
                case "/finished":
                    {
                        using StreamWriter writer = new(context.Response.OutputStream);
                        writer.Write("OK");
                        writer.Flush();

                        context.Response.Close();
                        break;
                    }
                default:
                    {
                        context.Response.StatusCode = 404;
                        break;
                    }
            }
        }

        private async Task AddLinks(Func<Comic, bool> autoStartFunc,
            HttpListener listener, CancellationToken ct)
        {
            progress = 0;
            foreach (var comic in stateRepository.Comics)
            {
                ct.ThrowIfCancellationRequested();
                await jdownloaderService.AddLinks(comic, autoStartFunc(comic), state =>
                {
                    jobState.UpdateState(state, false);
                    jobState.UpdateProgess(progress++, false);
                });

                await Task.Delay(1500, ct);
                HttpListenerContext context = await listener.GetContextAsync();
                HandleRequest(context, comic);
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
    }
}