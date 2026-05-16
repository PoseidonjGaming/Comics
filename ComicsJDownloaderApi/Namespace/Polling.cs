using JDownloader.Model;
using JDownloader.Namespace;
using System.Text.Json;

namespace ComicsJDownloaderApi.Namespace
{
    public class Polling(ComicsJDownloaderClient client) :
        BaseNamespace(client), IPolling
    {
        public override string Endpoint => "polling";

        public Task<PollingResult> Poll(APIQuery<object> query)
        {
            return PostRequestAsync<PollingResult>("poll", new object[1] { JsonSerializer.Serialize(query, base.JsonSerializerOptions) });
        }
    }
}