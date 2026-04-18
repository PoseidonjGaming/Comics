using DownloadComics.models;
using DownloadComics.resources.verify;
using DownloadComics.utilities;
using FuzzierSharp;
using HtmlAgilityPack;
using JDownloader;
using JDownloader.Model;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace DownloadComics.services
{
    public class JdownloaderService
    {
        private readonly Lazy<Task<JDownloaderClient>> _client = new(InitializeClientAsync,
            LazyThreadSafetyMode.ExecutionAndPublication);
        public Task<JDownloaderClient> GetInstanceAsync() => _client.Value;

        private static readonly Lazy<JdownloaderService> _instance = new(new JdownloaderService());
        public static readonly JdownloaderService Instance = _instance.Value;

        public int StartingCount = 0;
        private int counter = 0;
        private static AppState State
        {
            get
            {
                return AppStateStore.Instance;
            }
        }

        private JdownloaderService() { }


        private async static Task<JDownloaderClient> InitializeClientAsync()
        {
            JDownloaderClient client = new(new()
            {
                AppKey = "DownloadComics"
            });

            try
            {
                JDCredentials credentials = GetCredentials();
                if (!client.IsConnected)
                {
                    await client.Connect(credentials.Email, credentials.Password);

                    DeviceList devices = await client.ListDevices();
                    DeviceData? targetDevice = devices.Devices
                        .FirstOrDefault(d => d.Name == credentials.Device);

                    if (targetDevice != null)
                    {
                        client.SetWorkingDevice(targetDevice);

                        var directInfos = await client.Device.GetDirectConnectionInfos();
                        if (directInfos.Infos.Count > 0)
                        {
                            client.SetDirectConnectionInfo(directInfos.Infos[0]);
                        }
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                Dispatcher.CurrentDispatcher.Invoke(() => MessageBox.Show(ex.Message));
            }


            return client;
        }



        public static JDCredentials GetCredentials()
        {
            JDCredentials defaultCreds = new("", "", "");

            string? credentialBase64 = Properties.Settings.Default.JDCredentials;
            if (string.IsNullOrWhiteSpace(credentialBase64))
                return defaultCreds;

            try
            {
                byte[] bytes = Convert.FromBase64String(credentialBase64);
                byte[] unprotected = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
                string json = Encoding.UTF8.GetString(unprotected);

                return JsonConvert.DeserializeObject<JDCredentials>(json)
                    ?? defaultCreds;
            }
            catch
            {
                // Option : logger l'erreur ici
                return defaultCreds;
            }

        }

        public async Task AddLinks(Comic comic, bool autoStart, Action<string> stateAction)
        {
            JDownloaderClient client = await GetInstanceAsync();
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
            stateAction.Invoke($"{VerifyStrings.Verify_Add} {comic.PackageName}");
        }

        public async Task<List<CrawledLink>> GetCrawledLink(long? UUID = null)
        {
            JDownloaderClient client = await GetInstanceAsync();
            CrawledLinkQuery query = new()
            {
                Availability = true,
                Status = true,
                StartAt = 0,
                MaxResults = -1,
                Url = true,
                BytesTotal = true,
                JobUUIDs = UUID == null ? [.. State.GetComicsId()] : [UUID.Value],
            };
            return await client.LinkGrabberV2.QueryLinks(query);
        }

        public async Task<int> GetCrawledPackageCount()
        {
            JDownloaderClient client = await _client.Value;
            return await client.LinkGrabberV2.GetPackageCount();
        }

        public async Task SetCrawledPackageCount()
        {
            StartingCount = await GetCrawledPackageCount();
        }

        public async Task Reset()
        {
            counter = 0;
            await SetCrawledPackageCount();
        }

        public async Task<List<JobLinkCrawler>> GetCrawlJobs()
        {
            JDownloaderClient client = await GetInstanceAsync();
            return await client.LinkGrabberV2.QueryLinkCrawlerJobs(new(State.GetComicsId())
            {
                CollectorInfo = true
            });
        }

        public async Task RemoveLinks()
        {
            JDownloaderClient client = await GetInstanceAsync();
            List<CrawledLink> links = await GetCrawledLink();
            await client.LinkGrabberV2.RemoveLinks([.. links.Select(cl => cl.Uuid)], []);
        }

        public static void ChangeUrl(Comic comic, string[] hosts, Action<string> stateAction)
        {
            Track? track = State.GetTrackByUrl(comic.URL);

            if (track != null)
            {
                string? host = hosts.FirstOrDefault(h => !track.TestedHost.Contains(h));

                if (!string.IsNullOrEmpty(host))
                {
                    stateAction.Invoke($"{VerifyStrings.Verify_New_URL} {comic.PackageName}");

                    HtmlNode? node = ComicUtility.GetBodyNode(comic.BaseURL, comic.HtmlBody, out string? body);
                    if (node != null)
                    {
                        string? nextUrl = ComicUtility.GetUrlByHost(node, host);
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
                            State.RemoveComic(comic);
                        }
                    }
                }
                else
                {
                    State.RemoveTrack(track);
                    State.RemoveComic(comic);
                }
            }
        }

        public async Task<string?> GetComicJdownloader(string author, string name)
        {
            JDownloaderClient client = await GetInstanceAsync();

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

        public bool IsFinished(string count)
        {
            if (long.TryParse(count, out var uuid))
            {
                Comic? comic = State.GetComics().FirstOrDefault(c => c.UUID == uuid);
                if (comic != null)
                {
                    counter++;
                }
            }

            return counter == State.GetComics().Count;
        }
    }
}
