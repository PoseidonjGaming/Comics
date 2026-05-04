using ComicsLib.Models;
using FuzzierSharp;
using HtmlAgilityPack;
using JDownloader;
using JDownloader.Model;

namespace ComicsLib.Services
{
    public class JdownloaderService
    {
        private readonly Lazy<Task<JDownloaderClient>> _client;
        public Task<JDownloaderClient> GetClient()=> _client.Value;

        private static AppState State => AppStateStore.Instance;

        private readonly ReaderWriterLockSlim _startingCountLock;
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

        public JdownloaderService(Lazy<Task<JDownloaderClient>> jdClient) { 
            _client = jdClient;
            _startingCountLock = new();
        }

        public async Task<string?> GetComicJdownloader(string author, string name)
        {
            JDownloaderClient client = await GetClient();

            List<FilePackage> packages = await client.DownloadsV2.QueryPackages(new()
            {
                SaveTo = true
            });
            List<long> filePackages = [.. packages.Where(p => p.SaveTo.Contains(author)).Select(p => p.UUID)];

            List<DownloadLink> links = await client.DownloadsV2.QueryLinks(new());
            List<DownloadLink> filterLinks = [.. links.Where(dl => Fuzz.Ratio(Path.GetFileNameWithoutExtension(dl.Name), name)==100)
                .Where(dl => filePackages.Contains(dl.PackageUUID))];

            return filterLinks.FirstOrDefault()?.Comment;
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
            JDownloaderClient client = await _client.Value;
            return await client.LinkGrabberV2.GetPackageCount();
        }

        public async Task AddLinks(Comic comic, bool autoStart, Action<string> stateAction)
        {
            JDownloaderClient client = await GetClient();
            AddLinksQuery query = new()
            {
                AssignJobID = true,
                AutoStart = autoStart,
                DestinationFolder = comic.Path,
                Links = comic.URL,
                PackageName = comic.PackageName,
                Priority = Enum.Parse<JDownloader.Model.Priority>(comic.Priority.ToString()),
                OverwritePackagizerRules = true,
            };

            LinkCollectingJob job = await client.LinkGrabberV2.AddLinks(query);
            comic.UUID = job.Id;
            stateAction.Invoke($"Add {comic.PackageName}");
        }
        public static void ChangeUrl(Comic comic, string[] hosts, Action<string> stateAction)
        {
            Track? track = State.GetTrackByUrl(comic.URL);

            if (track != null)
            {
                string? host = hosts.FirstOrDefault(h => !track.TestedHost.Contains(h));

                if (!string.IsNullOrEmpty(host))
                {
                    stateAction.Invoke($"New url for {comic.PackageName}");

                    /*HtmlNode? node = ComicService.LoadBody(comic.HtmlBody);
                    if (node != null)
                    {
                        string? nextUrl = ComicService.GetUrlByHost(node, host);
                        if (!string.IsNullOrEmpty(nextUrl))
                        {

                            comic.URL = nextUrl;
                            comic.Host = host;
                            track.TestedHost.Add(host);
                            track.DownloadURL = nextUrl;
                        }
                        else
                        {
                            State.RemoveTrack(track);
                            State.Comics.Remove(comic);
                        }
                    }*/
                }
                else
                {
                    State.RemoveTrack(track);
                    State.Comics.Remove(comic);
                }
            }
        }

        public async Task<List<CrawledLink>> GetCrawledLink(long? UUID = null)
        {
            JDownloaderClient client = await GetClient();
            try
            {

                CrawledLinkQuery query = new()
                {
                    Availability = true,
                    Status = true,
                    StartAt = 0,
                    MaxResults = -1,
                    Url = true,
                    BytesTotal = true,
                    JobUUIDs = UUID == null ? [.. State.Comics.Select(c=>c.UUID)] : [UUID.Value],
                };
                return await client.LinkGrabberV2.QueryLinks(query);
            }
            catch (Exception)
            {
                foreach (var comic in State.Comics)
                {
                    try
                    {
                        CrawledLinkQuery query = new()
                        {
                            Availability = true,
                            Status = true,
                            StartAt = 0,
                            MaxResults = -1,
                            Url = true,
                            BytesTotal = true,
                            JobUUIDs = [comic.UUID],
                        };
                        List<CrawledLink> list = await client.LinkGrabberV2.QueryLinks(query);
                    }
                    catch (Exception ex)
                    {
                        //Dispatcher.CurrentDispatcher.Invoke(() => MessageBox.Show(ex.Message));
                    }

                }

                return [];
            }
        }

        public async Task RemoveLinks()
        {
            JDownloaderClient client = await GetClient();
            List<CrawledLink> links = await GetCrawledLink();
            await client.LinkGrabberV2.RemoveLinks([.. links.Select(cl => cl.Uuid)], []);
        }
    }
}
