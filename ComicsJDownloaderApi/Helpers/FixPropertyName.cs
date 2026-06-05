namespace ComicsJDownloaderApi.Helpers
{
    internal class FixPropertyName
    {
        private static readonly Dictionary<string, string> fixedProp = new() {
            { "availability", "AvailableLinkState" } };
        public static string Fix<T>(string json)
        {
            foreach (var kvp in fixedProp)
            {
                json = json.Replace(kvp.Key, kvp.Value);
            }
            return json;
        }
    }
}
