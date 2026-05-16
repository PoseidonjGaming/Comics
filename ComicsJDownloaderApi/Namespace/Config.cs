using JDownloader.Model;
using JDownloader.Namespace;
using System.Text.Json;

namespace ComicsJDownloaderApi.Namespace
{
    public class Config(ComicsJDownloaderClient client) :
        BaseNamespace(client), IConfig
    {
        public override string Endpoint => "config";
        public Task<object> Get(string interfaceName, string storageName, string key)
        {
            return PostRequestAsync<object>("get", new object[3] { interfaceName, storageName, key });
        }

        public Task<object> GetDefault(string interfaceName, string storageName, string key)
        {
            return PostRequestAsync<object>("getDefault", new object[3] { interfaceName, storageName, key });
        }

        public Task<IEnumerable<AdvancedConfigAPIEntry>> List(string pattern = null, bool returnDescription = false, bool returnValues = false, bool returnDefaultValues = false, bool returnEnumInfo = false)
        {
            return PostRequestAsync<IEnumerable<AdvancedConfigAPIEntry>>("list", new object[5] { pattern, returnDescription, returnValues, returnDefaultValues, returnEnumInfo });
        }

        public Task<IEnumerable<EnumOption>> ListEnum(string enumType)
        {
            return PostRequestAsync<IEnumerable<EnumOption>>("listEnum", new object[1] { enumType });
        }

        public Task<IEnumerable<AdvancedConfigAPIEntry>> Query(AdvancedConfigQuery query)
        {
            return PostRequestAsync<IEnumerable<AdvancedConfigAPIEntry>>("query", new object[1] { JsonSerializer.Serialize(query, base.JsonSerializerOptions) });
        }

        public Task<bool> Reset(string interfaceName, string storageName, string key)
        {
            return PostRequestAsync<bool>("reset", new object[3] { interfaceName, storageName, key });
        }

        public Task<bool> Set(string interfaceName, string storageName, string key, object value)
        {
            return PostRequestAsync<bool>("set", new object[4] { interfaceName, storageName, key, value });
        }
    }
}