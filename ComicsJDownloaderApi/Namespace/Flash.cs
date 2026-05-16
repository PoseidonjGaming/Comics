using JDownloader.Model;
using JDownloader.Namespace;
using System.Text.Json;

namespace ComicsJDownloaderApi.Namespace
{
    public class Flash(ComicsJDownloaderClient client) :
        BaseNamespace(client), IFlash
    {
        public override string Endpoint => "flash";

        public Task Add(string password, string source, string url)
        {
            return PostRequestAsync("add", new object[3] { password, source, url });
        }

        public Task AddCnl(CnlQuery query)
        {
            return PostRequestAsync("addcnl", new object[1] { JsonSerializer.Serialize(query, base.JsonSerializerOptions) });
        }

        public Task AddCrypted2Remote(string crypted, string jk, string k)
        {
            return PostRequestAsync("addcrypted2Remote", new object[3] { crypted, jk, k });
        }
    }
}