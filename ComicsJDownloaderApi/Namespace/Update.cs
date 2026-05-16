using JDownloader.Namespace;

namespace ComicsJDownloaderApi.Namespace
{
    public class Update(ComicsJDownloaderClient client) :
        BaseNamespace(client), IUpdate
    {
        public override string Endpoint => "update";

        public Task<bool> IsUpdateAvailable()
        {
            return PostRequestAsync<bool>("isUpdateAvailable");
        }

        public Task RestartAndUpdate()
        {
            return PostRequestAsync("restartAndUpdate", Array.Empty<object>());
        }

        public Task RunUpdateCheck()
        {
            return PostRequestAsync("runUpdateCheck", Array.Empty<object>());
        }
    }
}