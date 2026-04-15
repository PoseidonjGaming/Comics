using System.Net.Http;

namespace DownloadComics.utilities
{
    public class AdBlockHelper
    {
        private static readonly string[] BlockLists = {
        "https://raw.githubusercontent.com/uBlockOrigin/uAssets/master/filters/filters.txt",
        "https://easylist.to/easylist/easylist.txt",
        "https://easylist.to/easylist/easyprivacy.txt",
        "https://pgl.yoyo.org/adservers/serverlist.php?hostformat=hosts&showintro=0&mimetype=plaintext"
    };

        private static readonly List<string> Rules = new();

        public static async Task LoadRulesAsync()
        {
            if (Rules.Count > 0) return;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

            foreach (var url in BlockLists)
            {
                try
                {
                    var content = await client.GetStringAsync(url);
                    Rules.AddRange(content.Split('\n').Where(l => !l.StartsWith("!") && !string.IsNullOrWhiteSpace(l)));
                }
                catch { }
            }
        }

        public static bool IsAdOrTracking(string url)
        {
            if (Rules.Count == 0) return false; // si jamais pas encore chargé

            url = url.ToLowerInvariant();
            return Rules.Any(rule => url.Contains(rule.Trim().Replace("||", "").Replace("^", "")));
        }
    }
}
