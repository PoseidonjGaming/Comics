using JDownloader.Model;
using JDownloader.Namespace;

namespace ComicsJDownloaderApi.Namespace
{
    public class ContentV2(ComicsJDownloaderClient client) :
        BaseNamespace(client), IContentV2
    {
        public override string Endpoint => "contentV2";

        public Task<byte[]> GetFavIcon(string hosterName)
        {
            return PostRequestAsync<byte[]>("getFavIcon", new object[1] { hosterName });
        }

        public Task<byte[]> GetFileIcon(string fileName)
        {
            return PostRequestAsync<byte[]>("getFileIcon", new object[1] { fileName });
        }

        public Task<byte[]> GetIcon(string key, int size)
        {
            return PostRequestAsync<byte[]>("getIcon", new object[2] { key, size });
        }

        public Task<IconDescriptor> GetIconDescription(string key)
        {
            return PostRequestAsync<IconDescriptor>("getIconDescription", new object[1] { key });
        }
    }
}