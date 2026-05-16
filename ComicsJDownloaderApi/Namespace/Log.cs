using JDownloader.Model;
using JDownloader.Namespace;
using System.Text.Json;

namespace ComicsJDownloaderApi.Namespace
{
    public class Log(ComicsJDownloaderClient client) :
        BaseNamespace(client), ILog
    {
        public override string Endpoint => "log";

        public Task<List<LogFolder>> GetAvailableLogs()
        {
            return PostRequestAsync<List<LogFolder>>("getAvailableLogs");
        }

        public Task<string> SendLogFile(LogFolder[] logFolders)
        {
            return PostRequestAsync<string>("sendLogFile", new object[1] { JsonSerializer.Serialize(logFolders, base.JsonSerializerOptions) });
        }
    }
}