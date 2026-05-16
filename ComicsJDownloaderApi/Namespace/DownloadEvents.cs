using JDownloader.Model;
using JDownloader.Namespace;
using System.Text.Json;

namespace ComicsJDownloaderApi.Namespace
{
    public class DownloadEvents(ComicsJDownloaderClient client) : 
        BaseNamespace(client),IDownloadEvents
    {
        public override string Endpoint => "downloadevents";

        public Task<DownloadListDiff> QueryLinks(LinkQuery query, int diffId)
        {
            return PostRequestAsync<DownloadListDiff>("queryLinks", new object[2]
            {
            JsonSerializer.Serialize(query, base.JsonSerializerOptions),
            diffId
            });
        }

        public Task<bool> SetStatusEventInterval(long channelId, long interval)
        {
            return PostRequestAsync<bool>("setStatusEventInterval", new object[2] { channelId, interval });
        }
    }
}