using JDownloader.Model;
using JDownloader.Namespace;
using System.Text.Json;

namespace ComicsJDownloaderApi.Namespace
{
    public class Plugins(ComicsJDownloaderClient client) :
        BaseNamespace(client), IPlugins
    {
        public override string Endpoint => "plugins";

        public Task<object> Get(string interfaceName, string displayName, string key)
        {
            return PostRequestAsync<object>("get", new object[3] { interfaceName, displayName, key });
        }

        public Task<Dictionary<string, List<string>>> GetAllPluginRegex()
        {
            return PostRequestAsync<Dictionary<string, List<string>>>("getAllPluginRegex");
        }

        public Task<List<string>> GetPluginRegex(string url)
        {
            return PostRequestAsync<List<string>>("getPluginRegex", new object[1] { url });
        }

        public Task<List<Plugin>> List(PluginsQuery query)
        {
            return PostRequestAsync<List<Plugin>>("list", new object[1] { JsonSerializer.Serialize(query, base.JsonSerializerOptions) });
        }

        public Task<List<PluginConfigEntry>> Query(AdvancedConfigQuery query)
        {
            return PostRequestAsync<List<PluginConfigEntry>>("query", new object[1] { JsonSerializer.Serialize(query, base.JsonSerializerOptions) });
        }

        public Task<bool> Reset(string interfaceName, string displayName, string key)
        {
            return PostRequestAsync<bool>("reset", new object[3] { interfaceName, displayName, key });
        }

        public Task<bool> Set(string interfaceName, string displayName, string key, object newValue)
        {
            return PostRequestAsync<bool>("set", new object[4] { interfaceName, displayName, key, newValue });
        }
    }
}