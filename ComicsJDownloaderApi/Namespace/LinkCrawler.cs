using JDownloader.Namespace;

namespace ComicsJDownloaderApi.Namespace
{
    public class LinkCrawler(ComicsJDownloaderClient client) :
        BaseNamespace(client), ILinkCrawler
    {
        public Task<bool> IsCrawling()
        {
            return PostRequestAsync<bool>("isCrawling");
        }
    }
}