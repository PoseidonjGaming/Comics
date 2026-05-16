using JDownloader.Model;
using JDownloader.Namespace;
using System.Text.Json;

namespace ComicsJDownloaderApi.Namespace
{
    public class DownloadsV2(ComicsJDownloaderClient client) :
       BaseNamespace(client), IDownloadsV2
    {
        public override string Endpoint => "downloadsV2";

        public Task<bool> Cleanup(long[] linkIds, long[] packageIds, CleanupAction cleanupAction, CleanupMode cleanupMode, SelectionType selectionType)
        {
            return PostRequestAsync<bool>("cleanup", new object[5]
            {
            linkIds,
            packageIds,
            cleanupAction.ToString(),
            cleanupMode.ToString(),
            selectionType.ToString()
            });
        }

        public Task<bool> ForceDownload(long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync<bool>("forceDownload", new object[2] { linkIds, packageIds });
        }

        public Task<Dictionary<string, List<long>>> GetDownloadUrls(long[] linkIds, long[] packageIds, UrlDisplayType[] urlDisplayTypes)
        {
            return PostRequestAsync<Dictionary<string, List<long>>>("getDownloadUrls", new object[3]
            {
            linkIds,
            packageIds,
            urlDisplayTypes.Select((UrlDisplayType x) => x.ToString()).ToArray()
            });
        }

        public Task<long> GetStopMark()
        {
            return PostRequestAsync<long>("getStopMark");
        }

        public Task<DownloadLink> GetStopMarkedLink()
        {
            return PostRequestAsync<DownloadLink>("getStopMarkedLink");
        }

        public Task<long> GetStructureChangeCounter(long oldCounterValue)
        {
            return PostRequestAsync<long>("getStructureChangeCounter", new object[1] { oldCounterValue });
        }

        public Task MoveLinks(long[] linkIds, long afterLinkId, long targetPackageId)
        {
            return PostRequestAsync("moveLinks", new object[3] { linkIds, afterLinkId, targetPackageId });
        }

        public Task MovePackages(long[] packageIds, long targetPackageId)
        {
            return PostRequestAsync("movePackages", new object[2] { packageIds, targetPackageId });
        }

        public Task MoveToNewPackage(long[] linkIds, long[] packageIds, string newPackageName, string downloadPath)
        {
            return PostRequestAsync("movetoNewPackage", new object[4] { linkIds, packageIds, newPackageName, downloadPath });
        }

        public Task<int> PackageCount()
        {
            return PostRequestAsync<int>("packageCount");
        }

        public Task<List<DownloadLink>> QueryLinks(LinkQuery query)
        {
            return PostRequestAsync<List<DownloadLink>>("queryLinks", new object[1] { JsonSerializer.Serialize(query, base.JsonSerializerOptions) });
        }

        public Task<List<FilePackage>> QueryPackages(PackageQuery query)
        {
            return PostRequestAsync<List<FilePackage>>("queryPackages", new object[1] { JsonSerializer.Serialize(query, base.JsonSerializerOptions) });
        }

        public Task RemoveLinks(long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync("removeLinks", new object[2] { linkIds, packageIds });
        }

        public Task RemoveStopMark()
        {
            return PostRequestAsync("removeStopMark", Array.Empty<object>());
        }

        public Task RenameLink(long linkId, string newName)
        {
            return PostRequestAsync("renameLink", new object[2] { linkId, newName });
        }

        public Task RenamePackage(long packageId, string newName)
        {
            return PostRequestAsync("renamePackage", new object[2] { packageId, newName });
        }

        public Task ResetLinks(long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync("resetLinks", new object[2] { linkIds, packageIds });
        }

        public Task ResumeLinks(long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync("resumeLinks", new object[2] { linkIds, packageIds });
        }

        public Task SetComment(long[] linkIds, long[] packageIds, bool setPackageChildren, string comment)
        {
            return PostRequestAsync("setComment", new object[4] { linkIds, packageIds, setPackageChildren, comment });
        }

        public Task SetDownloadDirectory(string directory, long[] packageIds)
        {
            return PostRequestAsync("setDownloadDirectory", new object[2] { directory, packageIds });
        }

        public Task<bool> SetDownloadPassword(long[] linkIds, long[] packageIds, string pass)
        {
            return PostRequestAsync<bool>("setDownloadPassword", new object[3] { linkIds, packageIds, pass });
        }

        public Task SetEnabled(bool enabled, long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync("setEnabled", new object[3] { enabled, linkIds, packageIds });
        }

        public Task SetPriority(Priority priority, long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync("setPriority", new object[3]
            {
            priority.ToString(),
            linkIds,
            packageIds
            });
        }

        public Task SetStopMark(long linkId, long packageId)
        {
            return PostRequestAsync("setStopMark", new object[2] { linkId, packageId });
        }

        public Task SplitPackageByHoster(long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync("splitPackageByHoster", new object[2] { linkIds, packageIds });
        }

        public Task StartOnlineStatusCheck(long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync("startOnlineStatusCheck", new object[2] { linkIds, packageIds });
        }

        public Task Unskip(long[] packageIds, long[] linkIds, Reason filterByReason)
        {
            return PostRequestAsync("unskip", new object[3]
            {
            packageIds,
            linkIds,
            filterByReason.ToString()
            });
        }
    }
}