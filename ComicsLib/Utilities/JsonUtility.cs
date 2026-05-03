using System.Text.Json.Nodes;

namespace ComicsLib.Utilities
{
    public class JsonUtility
    {
        public static List<string> GetURLS(string json)
        {
            List<string> urls = [];
            JsonNode? root = JsonNode.Parse(json);
            if (root != null)
            {
                BrowseNode(root.AsArray(), urls);
            }
            return urls;
        }

        private static void BrowseObject(JsonObject node, List<string> urls)
        {
            foreach (var prop in node)
            {
                if (prop.Value != null)
                {
                    SwitchJson(prop.Value, urls);
                }
            }
        }

        private static void BrowseNode(JsonArray array, List<string> urls)
        {
            foreach (JsonNode? node in array)
            {
                if (node != null)
                {
                    SwitchJson(node, urls);
                }
            }
        }

        private static void SwitchJson(JsonNode node, List<string> urls)
        {
            switch (node)
            {
                case JsonObject obj:
                    BrowseObject(obj, urls);
                    break;
                case JsonArray arr:
                    BrowseNode(arr, urls);
                    break;
                case JsonValue val:
                    if (val.TryGetValue<string>(out string? str) && str.StartsWith("http") && RegexUtility.HostRegex().IsMatch(str))
                    {
                        urls.Add(str);
                    }
                    break;
            }
        }

        
    }
}
