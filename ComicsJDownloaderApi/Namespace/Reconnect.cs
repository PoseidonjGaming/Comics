using JDownloader.Namespace;

namespace ComicsJDownloaderApi.Namespace
{
    public class Reconnect(ComicsJDownloaderClient client) :
        BaseNamespace(client), IReconnect
    {
        public override string Endpoint => "reconnect";

        public Task DoReconnect()
        {
            return PostRequestAsync("doReconnect", Array.Empty<object>());
        }
    }
}