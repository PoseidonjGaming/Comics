using System.Text.Json;
using System.Text.Json.Serialization;

namespace ComicsJDownloaderApi.Helpers
{
    internal class JDownloaderUtility
    {
        public static HttpClient CreateHttpClient()
        {
            return new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(120.0)
            };
        }

        public static JsonSerializerOptions CreateJsonSerializerOptions()
        {
            return new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                Converters = { new JsonStringEnumConverter() }
            };
        }

        public static string GetServerUrl(string action)
        {
            return "https://api.jdownloader.org" + action;
        }

        public static long GetUniqueRequestId()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public static string DoubleJsonDecode(string json)
        {
            return json.Replace("\\r", string.Empty).Replace("\\n", Environment.NewLine).Replace("\\\"", "\"")
                .Replace("\\", string.Empty)
                .Replace("\"{", "{")
                .Replace("}\"", "}")
                .Replace("\"[", "[")
                .Replace("]\"", "]");
        }
    }
}
