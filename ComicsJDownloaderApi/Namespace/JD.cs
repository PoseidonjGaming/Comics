using JDownloader.Namespace;

namespace ComicsJDownloaderApi.Namespace
{
    [Obsolete("This namespace has been marked as deprecated in JD source.")]
    public class JD(ComicsJDownloaderClient client) :
        BaseNamespace(client), IJD
    {
        public override string Endpoint => "jd";

        public Task<int> GetCoreRevision()
        {
            return PostRequestAsync<int>("getCoreRevision");
        }

        public Task<bool> RefreshPlugins()
        {
            return PostRequestAsync<bool>("refreshPlugins");
        }

        public Task<long> Uptime()
        {
            return PostRequestAsync<long>("uptime");
        }

        public Task<long> Version()
        {
            return PostRequestAsync<long>("version");
        }
    }
}