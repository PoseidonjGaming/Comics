using JDownloader.Model;
using JDownloader.Namespace;

namespace ComicsJDownloaderApi.Namespace
{
    public class DownloadController(ComicsJDownloaderClient client) :
        BaseNamespace(client), IDownloadController
    {
        public override string Endpoint => "downloadcontroller";

        public Task ForceDownload(long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync("forceDownload", new object[2] { linkIds, packageIds });
        }

        public Task<DownloaderState> GetCurrentState()
        {
            return PostRequestAsync<DownloaderState>("getCurrentState");
        }

        public Task<long> GetSpeedInBps()
        {
            return PostRequestAsync<long>("getSpeedInBps");
        }

        public Task<bool> Start()
        {
            return PostRequestAsync<bool>("start");
        }

        public Task<bool> Stop()
        {
            return PostRequestAsync<bool>("stop");
        }

        public Task<bool> Pause(bool pause)
        {
            return PostRequestAsync<bool>("pause", new object[1] { pause });
        }
    }
}