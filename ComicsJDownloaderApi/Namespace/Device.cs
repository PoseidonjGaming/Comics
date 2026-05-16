using JDownloader.Model;
using JDownloader.Namespace;

namespace ComicsJDownloaderApi.Namespace
{
    public class Device(ComicsJDownloaderClient client) : BaseNamespace(client), IDevice
    {
        public override string Endpoint => "device";
        public Task<DirectConnectionInfos> GetDirectConnectionInfos()
        {
            return PostRequestAsync<DirectConnectionInfos>("getDirectConnectionInfos");
        }

        public Task<string> GetSessionPublicKey()
        {
            return PostRequestAsync<string>("getSessionPublicKey");
        }

        public Task<bool> Ping()
        {
            return PostRequestAsync<bool>("ping");
        }
    }
}
