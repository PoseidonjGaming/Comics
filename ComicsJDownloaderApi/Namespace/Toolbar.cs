using JDownloader.Model;
using JDownloader.Namespace;

namespace ComicsJDownloaderApi.Namespace
{
    public class Toolbar(ComicsJDownloaderClient client) :
        BaseNamespace(client), IToolbar
    {
        public override string Endpoint => "toolbar";

        public Task<object> AddLinksFromDOM()
        {
            return PostRequestAsync<object>("addLinksFromDOM");
        }

        public Task<object> CheckLinksFromDOM()
        {
            return PostRequestAsync<object>("checkLinksFromDOM");
        }

        public Task<object> GetStatus()
        {
            return PostRequestAsync<object>("getStatus");
        }

        public Task<bool> IsAvailable()
        {
            return PostRequestAsync<bool>("isAvailable");
        }

        public Task<LinkCheckResult> PollCheckedLinksFromDOM(string checkId)
        {
            return PostRequestAsync<LinkCheckResult>("pollCheckedLinksFromDOM", new object[1] { checkId });
        }

        public Task<string> SpecialURLHandling(string url)
        {
            return PostRequestAsync<string>("specialURLHandling", new object[1] { url });
        }

        public Task<bool> StartDownloads()
        {
            return PostRequestAsync<bool>("startDownloads");
        }

        public Task<bool> StopDownloads()
        {
            return PostRequestAsync<bool>("stopDownloads");
        }

        public Task<bool> ToggleAutomaticReconnect()
        {
            return PostRequestAsync<bool>("toggleAutomaticReconnect");
        }

        public Task<bool> ToggleClipboardMonitoring()
        {
            return PostRequestAsync<bool>("toggleClipboardMonitoring");
        }

        public Task<bool> ToggleDownloadSpeedLimit()
        {
            return PostRequestAsync<bool>("toggleDownloadSpeedLimit");
        }

        public Task<bool> TogglePauseDownloads()
        {
            return PostRequestAsync<bool>("togglePauseDownloads");
        }

        public Task<bool> TogglePremium()
        {
            return PostRequestAsync<bool>("togglePremium");
        }

        public Task<bool> ToggleStopAfterCurrentDownload()
        {
            return PostRequestAsync<bool>("toggleStopAfterCurrentDownload");
        }

        public Task<bool> TriggerUpdate()
        {
            return PostRequestAsync<bool>("triggerUpdate");
        }
    }
}