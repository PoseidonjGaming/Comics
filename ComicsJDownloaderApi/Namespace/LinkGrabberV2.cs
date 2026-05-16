using JDownloader.Model;
using JDownloader.Namespace;
using System.Text.Json;

namespace ComicsJDownloaderApi.Namespace
{
    public class LinkGrabberV2(ComicsJDownloaderClient client): 
        BaseNamespace(client), ILinkGrabberV2
    {
        public override string Endpoint => "linkgrabberv2";

        public Task<bool> Abort()
        {
            return PostRequestAsync<bool>("abort");
        }

        public Task<bool> Abort(long jobId)
        {
            return PostRequestAsync<bool>("abort", new object[1] { jobId });
        }

        public Task<LinkCollectingJob> AddContainer(string type, string content)
        {
            return PostRequestAsync<LinkCollectingJob>("addContainer", new object[2] { type, content });
        }

        public Task<LinkCollectingJob> AddLinks(AddLinksQuery query)
        {
            return PostRequestAsync<LinkCollectingJob>("addLinks",
                new object[1] { JsonSerializer.Serialize(query, base.JsonSerializerOptions) });
        }

        public Task AddVariantCopy(long linkId, long targetLinkId, long targetPackageId, string variantId)
        {
            return PostRequestAsync("addVariantCopy", new object[4] { linkId, targetLinkId, targetPackageId, variantId });
        }

        public Task Cleanup(long[] linkIds, long[] packageIds, CleanupAction cleanupAction, CleanupMode cleanbupMode, SelectionType selectionType)
        {
            return PostRequestAsync("cleanup", new object[5]
            {
            linkIds,
            packageIds,
            cleanupAction.ToString(),
            cleanbupMode.ToString(),
            selectionType.ToString()
            });
        }

        public Task<bool> ClearList()
        {
            return PostRequestAsync<bool>("clearList");
        }

        public Task<long> GetChildrenChanged(long structureWatermark)
        {
            return PostRequestAsync<long>("getChildrenChanged", new object[1] { structureWatermark });
        }

        public Task<List<string>> GetDownloadFolderHistorySelectionBase()
        {
            return PostRequestAsync<List<string>>("getDownloadFolderHistorySelectionBase");
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

        public Task<int> GetPackageCount()
        {
            return PostRequestAsync<int>("getPackageCount");
        }

        public Task<List<LinkVariant>> GetVariants(long linkId)
        {
            return PostRequestAsync<List<LinkVariant>>("getVariants", new object[1] { linkId });
        }

        public Task<bool> IsCollecting()
        {
            return PostRequestAsync<bool>("isCollecting");
        }

        public Task MoveLinks(long[] linkIds, long targetLinkId, long targetPackageId)
        {
            return PostRequestAsync("moveLinks", new object[3] { linkIds, targetLinkId, targetPackageId });
        }

        public Task MovePackages(long[] packageIds, long targetPackageId)
        {
            return PostRequestAsync("movePackages", new object[2] { packageIds, targetPackageId });
        }

        public Task MoveToDownloadList(long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync("moveToDownloadlist", new object[2] { linkIds, packageIds });
        }

        public Task MoveToNewPackage(long[] linkIds, long[] packageIds, string newPackageName, string downloadPath)
        {
            return PostRequestAsync("movetoNewPackage", new object[4] { linkIds, packageIds, newPackageName, downloadPath });
        }

        public Task<List<JobLinkCrawler>> QueryLinkCrawlerJobs(LinkCrawlerJobsQuery query)
        {
            return PostRequestAsync<List<JobLinkCrawler>>("queryLinkCrawlerJobs", 
                new object[1] { JsonSerializer.Serialize(query, base.JsonSerializerOptions) });
        }

        public Task<List<CrawledLink>> QueryLinks(CrawledLinkQuery query)
        {
            return PostRequestAsync<List<CrawledLink>>("queryLinks", 
                new object[1] { JsonSerializer.Serialize(query, base.JsonSerializerOptions) });
        }

        public Task<List<CrawledPackage>> QueryPackages(CrawledPackageQuery query)
        {
            return PostRequestAsync<List<CrawledPackage>>("queryPackages",
                new object[1] { JsonSerializer.Serialize(query, base.JsonSerializerOptions) });
        }

        public Task RemoveLinks(long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync("removeLinks", new object[2] { linkIds, packageIds });
        }

        public Task RenameLink(long linkId, string newName)
        {
            return PostRequestAsync("renameLink", new object[2] { linkId, newName });
        }

        public Task RenamePackage(long packageId, string newName)
        {
            return PostRequestAsync("renamePackage", new object[2] { packageId, newName });
        }

        public Task SetComment(long[] linkIds, long[] packageIds, bool setPackageChildren, string comment)
        {
            return PostRequestAsync("setComment", new object[4] { linkIds, packageIds, setPackageChildren, comment });
        }

        public Task SetDownloadDirectory(long[] packageIds, string downloadPath)
        {
            return PostRequestAsync("setDownloadDirectory", new object[2] { downloadPath, packageIds });
        }

        public Task<bool> SetDownloadPassword(long[] linkIds, long[] packageIds, string password)
        {
            return PostRequestAsync<bool>("setDownloadPassword", new object[3] { linkIds, packageIds, password });
        }

        public Task SetEnabled(long[] linkIds, long[] packageIds, bool enabled)
        {
            return PostRequestAsync("setEnabled", new object[3] { enabled, linkIds, packageIds });
        }

        public Task SetPriority(long[] linkIds, long[] packageIds, Priority priority)
        {
            return PostRequestAsync("setPriority", new object[3] { priority, linkIds, packageIds });
        }

        public Task SetVariant(long linkId, string variantId)
        {
            return PostRequestAsync("setVariant", new object[2] { linkId, variantId });
        }

        public Task SplitPackageByHoster(long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync("splitPackageByHoster", new object[2] { linkIds, packageIds });
        }

        public Task StartOnlineStatusCheck(long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync("startOnlineStatusCheck", new object[2] { linkIds, packageIds });
        }
    }
}
