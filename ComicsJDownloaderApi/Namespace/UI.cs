using JDownloader.Model;
using JDownloader.Namespace;
using System.Text.Json;

namespace ComicsJDownloaderApi.Namespace
{
    public class UI(ComicsJDownloaderClient client) :
        BaseNamespace(client), IUI
    {
        public override string Endpoint => "ui";

        public Task<MyJDMenuItem> GetMenu(Context context)
        {
            return PostRequestAsync<MyJDMenuItem>("getMenu", new object[1] { JsonSerializer.Serialize(context, base.JsonSerializerOptions) });
        }

        public Task<object> InvokeAction(Context context, string id, long[] linkIds, long[] packageIds)
        {
            return PostRequestAsync<object>("invokeAction", new object[4]
            {
            JsonSerializer.Serialize(context, base.JsonSerializerOptions),
            id,
            linkIds,
            packageIds
            });
        }
    }
}