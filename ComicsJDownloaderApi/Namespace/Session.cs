using JDownloader.Namespace;

namespace ComicsJDownloaderApi.Namespace
{
    public class Session(ComicsJDownloaderClient client) :
        BaseNamespace(client), ISession
    {
        public override string Endpoint => "session";

        public Task<bool> Disconnect()
        {
            return PostRequestAsync<bool>("disconnect");
        }

        public Task<string> Handshake(string username, string password)
        {
            return PostRequestAsync<string>("handshake", new object[2] { username, password });
        }
    }
}