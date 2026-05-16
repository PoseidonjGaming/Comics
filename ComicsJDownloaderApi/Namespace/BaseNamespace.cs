using ComicsJDownloaderApi.Model;
using System.Text.Json;

namespace ComicsJDownloaderApi.Namespace
{
    public abstract class BaseNamespace(ComicsJDownloaderClient jDownloaderClient)
    {
        protected readonly ComicsJDownloaderClient _jDownloaderClient = jDownloaderClient;
        protected JsonSerializerOptions JsonSerializerOptions => _jDownloaderClient.SerializerOptions;

        public virtual string Endpoint { get; } = string.Empty;

        protected Task PostRequestAsync(string action, object param, bool doubleJsonEncode = false, bool skipDeserialization = false)
        {
            return _jDownloaderClient.PostRequestAsync<object>(new PostRequestArg
            {
                Action = "/" + Endpoint + "/" + action,
                Param = param,
                DoubleJsonDecode = doubleJsonEncode,
                SkipDeserialization = skipDeserialization
            });
        }

        protected Task<T> PostRequestAsync<T>(string action, object? param = null,
            bool doubleJsonEncode = false, bool skipDeserialization = false)
        {
            return _jDownloaderClient.PostRequestAsync<T>(new PostRequestArg
            {
                Action = "/" + Endpoint + "/" + action,
                Param = param,
                DoubleJsonDecode = doubleJsonEncode,
                SkipDeserialization = skipDeserialization
            });
        }
    }
}
