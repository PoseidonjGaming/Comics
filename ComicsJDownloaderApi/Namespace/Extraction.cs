using JDownloader.Model;
using JDownloader.Namespace;
using System.Text.Json;

namespace ComicsJDownloaderApi.Namespace
{
    public class Extraction(ComicsJDownloaderClient client) :
        BaseNamespace(client), IExtraction
    {
        public override string Endpoint => "extraction";

        public Task AddArchivePassword(string password)
        {
            return PostRequestAsync("addArchivePassword", new object[1] { password });
        }

        public Task<bool> CancelExtraction(long controllerId)
        {
            return PostRequestAsync<bool>("cancelExtraction", new object[1] { controllerId });
        }

        public Task<List<ArchiveStatus>> GetArchiveInfo(long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync<List<ArchiveStatus>>("getArchiveInfo", new object[2] { linkIds, packageIds });
        }

        public Task<List<ArchiveSettings>> GetArchiveSettings(string[] archiveIds)
        {
            return PostRequestAsync<List<ArchiveSettings>>("getArchiveSettings", new object[1] { archiveIds });
        }

        public Task<List<ArchiveStatus>> GetQueue()
        {
            return PostRequestAsync<List<ArchiveStatus>>("getQueue");
        }

        public Task<bool> SetArchiveSettings(string archiveId, ArchiveSettings archiveSettings)
        {
            return PostRequestAsync<bool>("setArchiveSettings", new object[2]
            {
            archiveId,
            JsonSerializer.Serialize(archiveSettings, base.JsonSerializerOptions)
            });
        }

        public Task<Dictionary<string, bool?>> StartExtractionNow(long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync<Dictionary<string, bool?>>("startExtractionNow", new object[2] { linkIds, packageIds });
        }
    }
}