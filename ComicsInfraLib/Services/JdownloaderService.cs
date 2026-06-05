using ComicsJDownloaderApi;
using ComicsLib.Models;
using ComicsLocalizationLib;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using FuzzierSharp;
using HtmlAgilityPack;
using JDownloader;
using JDownloader.Model;

namespace ComicsInfraLib.Services
{
    public class JdownloaderService(Lazy<Task<ComicsJDownloaderClient>> jdClient,
        IHtmlParserService htmlParserService, IStateRepository stateRepository, 
        LocalizationService localizationService)
    {
        public Task<ComicsJDownloaderClient> GetClient() => jdClient.Value;

        private readonly ReaderWriterLockSlim _startingCountLock = new();
        private int _startingCount;
        public int StartingCount
        {
            get
            {
                _startingCountLock.EnterReadLock();
                try
                {
                    return _startingCount;
                }
                finally
                {
                    _startingCountLock.ExitReadLock();
                }
            }

            set
            {
                _startingCountLock.EnterWriteLock();
                try
                {
                    _startingCount = value;
                }
                finally
                {
                    _startingCountLock.ExitWriteLock();
                }
            }

        }

        public async Task<string?> GetComicJdownloader(string author, string name)
        {
            try
            {
                ComicsJDownloaderClient client = await GetClient();

                List<FilePackage> packages = await client.DownloadsV2.QueryPackages(new()
                {
                    SaveTo = true,
                    MaxResults = -1,
                    StartAt = 0
                });
                List<long> filePackages = [.. packages.Where(p => p.SaveTo.Contains(author)).Select(p => p.UUID)];

                List<DownloadLink> links = await client.DownloadsV2.QueryLinks(new()
                {
                    MaxResults = -1,
                    StartAt = 0
                });
                List<DownloadLink> filterLinks = [.. links.Where(dl =>
                Fuzz.Ratio(Path.GetFileNameWithoutExtension(dl.Name).ToLower(), name.ToLower())==100)
                .Where(dl => filePackages.Contains(dl.PackageUUID))];

                return filterLinks.FirstOrDefault()?.Comment;
            }
            catch
            {
                return "JDownloader not open";
            }

        }

        public async Task Reset()
        {
            await SetCrawledPackageCount();
        }

        public async Task SetCrawledPackageCount()
        {
            StartingCount = await GetCrawledPackageCount();
        }

        public async Task<int> GetCrawledPackageCount()
        {
            ComicsJDownloaderClient client = await jdClient.Value;
            return await client.LinkGrabberV2.GetPackageCount();
        }

        public async Task AddLinks(Comic comic, bool autoStart, Action<string> stateAction)
        {
            ComicsJDownloaderClient client = await GetClient();
            AddLinksQuery query = new()
            {
                AssignJobID = true,
                AutoStart = autoStart,
                DestinationFolder = comic.Path,
                Links = comic.URL,
                PackageName = comic.PackageName,
                Priority = Enum.Parse<Priority>(comic.Priority.ToString()),
                OverwritePackagizerRules = true,
            };

            LinkCollectingJob job = await client.LinkGrabberV2.AddLinks(query);
            comic.UUID = job.Id;
            stateAction.Invoke($"{localizationService["SendPage.Add"]} {comic.PackageName}");
        }
        public void ChangeUrl(Comic comic, string[] hosts, Action<string> stateAction)
        {
            Track? track = stateRepository.Tracks.FirstOrDefault(t => t.DownloadURL == comic.URL);

            if (track != null && !string.IsNullOrEmpty(comic.HtmlBody))
            {
                string? host = hosts.FirstOrDefault(h => !track.TestedHost.Contains(h));

                if (!string.IsNullOrEmpty(host))
                {
                    stateAction.Invoke($"New url for {comic.PackageName}");

                    HtmlNode? bodyNode = htmlParserService.LoadBody(comic.HtmlBody);
                    if (bodyNode != null)
                    {
                        HtmlNode? node = htmlParserService.FindNodeWithAttribute(bodyNode, host, "href");
                        string? nextUrl = node?.GetAttributeValue("href", "no value");
                        if (!string.IsNullOrEmpty(nextUrl))
                        {
                            comic.URL = nextUrl;
                            comic.Host = host;
                            track.TestedHost.Add(host);
                            track.DownloadURL = nextUrl;
                        }
                        else
                        {
                            stateRepository.Tracks.Remove(track);
                            stateRepository.Comics.Remove(comic);
                        }
                    }
                }
                else
                {
                    stateRepository.Tracks.Remove(track);
                    stateRepository.Comics.Remove(comic);
                }
            }
        }

        public async Task<List<CrawledLink>> GetCrawledLink(long? UUID = null)
        {
            ComicsJDownloaderClient client = await GetClient();

            CrawledLinkQuery query = new()
            {
                Availability = true,
                Status = true,
                StartAt = 0,
                MaxResults = -1,
                Url = true,
                BytesTotal = true,
                JobUUIDs = UUID == null ? [.. stateRepository.Comics.Select(c => c.UUID)]
                    : [UUID.Value],
            };
            return await client.LinkGrabberV2.QueryLinks(query);

        }

        public async Task RemoveLinks()
        {
            ComicsJDownloaderClient client = await GetClient();
            List<CrawledLink> links = await GetCrawledLink();
            await client.LinkGrabberV2.RemoveLinks([.. links.Select(cl => cl.Uuid)], []);
        }
    }
}
