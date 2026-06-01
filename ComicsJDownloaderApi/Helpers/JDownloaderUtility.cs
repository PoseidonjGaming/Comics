using JDownloader.Model;
using JDownloader.Model.Enum;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

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
                Converters = {
                    new EnumJsonConverter<AvailableLinkState>(),
                new EnumJsonConverter<AccountErrorType>(),
                new EnumJsonConverter<ArchiveFileStatus>(),
                new EnumJsonConverter<CaptchaSkipRequest>(),
                new EnumJsonConverter<CleanupAction>(),
                new EnumJsonConverter<CleanupMode>(),
                new EnumJsonConverter<Context>(),
                new EnumJsonConverter<ControllerStatus>(),
                new EnumJsonConverter<DeviceStatus>(),
                new EnumJsonConverter<DirectConnectionMode>(),
                new EnumJsonConverter<ErrorSource>(),
                new EnumJsonConverter<HostType>(),
                new EnumJsonConverter<LinkCheckStatus>(),
                new EnumJsonConverter<MenuType>(),
                new EnumJsonConverter<Priority>(),
                new EnumJsonConverter<Reason>(),
                new EnumJsonConverter<SelectionType>(),
                new EnumJsonConverter<UrlDisplayType>()}
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
