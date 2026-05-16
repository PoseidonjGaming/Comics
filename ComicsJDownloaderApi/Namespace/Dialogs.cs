using JDownloader.Model;
using JDownloader.Namespace;

namespace ComicsJDownloaderApi.Namespace
{
    public class Dialogs(ComicsJDownloaderClient client) :
        BaseNamespace(client), IDialogs
    {
        public override string Endpoint => "dialogs";

        public Task Answer(long dialogId, Dictionary<string, object> dialogAnswer)
        {
            return PostRequestAsync("answer", new object[2] { dialogId, dialogAnswer });
        }

        public Task<DialogInfo> Get(long dialogId, bool icon, bool properties)
        {
            return PostRequestAsync<DialogInfo>("get", new object[3] { dialogId, icon, properties });
        }

        public Task<DialogTypeInfo> GetTypeInfo(string dialogType)
        {
            return PostRequestAsync<DialogTypeInfo>("getTypeInfo", new object[1] { dialogType });
        }

        public Task<long[]> List()
        {
            return PostRequestAsync<long[]>("list");
        }
    }
}