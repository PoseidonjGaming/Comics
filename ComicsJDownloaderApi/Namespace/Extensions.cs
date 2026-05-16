using JDownloader.Model;
using JDownloader.Namespace;
using System.Text.Json;

namespace ComicsJDownloaderApi.Namespace
{
    public class Extensions(ComicsJDownloaderClient client) :
        BaseNamespace(client), IExtensions
    {
        public override string Endpoint => "extensions";

        public Task<bool> Install(string extensionId)
        {
            return PostRequestAsync<bool>("install", new object[1] { extensionId });
        }

        public Task<bool> IsEnabled(string className)
        {
            return PostRequestAsync<bool>("isEnabled", new object[1] { className });
        }

        public Task<bool> IsInstalled(string extensionId)
        {
            return PostRequestAsync<bool>("isInstalled", new object[1] { extensionId });
        }

        public Task<List<Extension>> List(ExtensionQuery query)
        {
            return PostRequestAsync<List<Extension>>("list", new object[1] { JsonSerializer.Serialize(query, base.JsonSerializerOptions) });
        }

        public Task<bool> SetEnabled(string className, bool enabled)
        {
            return PostRequestAsync<bool>("setEnabled", new object[2] { className, enabled });
        }
    }
}