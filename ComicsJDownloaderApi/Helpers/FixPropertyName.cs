namespace ComicsJDownloaderApi.Helpers
{
    internal class FixPropertyName
    {
        public static string Fix<T>(string json)
        {
           return json.Replace("availability", "AvailableLinkState");
        }
    }
}
