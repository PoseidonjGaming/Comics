using ComicsLocalizationLib.Resources;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ComicsLocalizationLib
{
    public partial class LocalizationService : INotifyPropertyChanged
    {
        private Dictionary<string, string> _data = [];
        public Dictionary<string, string> Data { get => _data; }
        public string CurrentCulture { get; private set; } = "en";

        public List<LanguageOption> Languages { get; }

        public string this[string key] => Get(key);

        public event PropertyChangedEventHandler? PropertyChanged;

        public LocalizationService()
        {
            Languages = [..Assembly.GetExecutingAssembly().GetManifestResourceNames().
                Where(s=>s.Contains("Langs")).Select(s=>{
                string code = s.Split("Langs").Last().TrimStart('.').Replace(".json", string.Empty);
                return new LanguageOption(code,code);
            })];
        }

        public string Get(string key)
        {
            return _data.TryGetValue(key, out var value) ? value : $"#{key}#";
        }

        public void LoadLang(string lang)
        {
            string fileResource = GetRessource($"{lang}.json");

            using Stream? stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(fileResource)
                ?? throw new Exception("The stream is empty");

            using StreamReader reader = new(stream);
            string json = reader.ReadToEnd();
            _data = Deserializer(json);

            CurrentCulture = lang;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));

        }

        private static Dictionary<string, string> Deserializer(string json)
        {
            Dictionary<string, string> rootDic = [];

            JsonNode? root = JsonNode.Parse(json);
            if (root != null)
            {
                foreach (var rootProp in root.AsObject())
                {
                    if (rootProp.Value?.GetValueKind() == JsonValueKind.Object)
                    {
                        BrowseNode(rootProp.Value.AsObject(), rootProp.Key, rootDic);
                    }

                }
            }

            return rootDic;
        }

        private static void BrowseNode(JsonObject node, string key,
            Dictionary<string, string> dic)
        {
            foreach (var prop in node)
            {
                if (prop.Value?.GetValueKind() == JsonValueKind.Object)
                {
                    BrowseNode(prop.Value.AsObject(), $"{key}.{prop.Key}", dic);

                }
                else if (prop.Value?.GetValueKind() == JsonValueKind.String)
                {
                    dic[$"{key}.{prop.Key}"] = prop.Value?.ToString() ?? "";
                }
            }
        }

        private static string GetRessource(string lang)
        {
            var list = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            return Assembly.GetExecutingAssembly()
                 .GetManifestResourceNames().First(s =>
                 s.Equals($"ComicsLocalizationLib.Resources.Langs.{lang}"));
        }

        public static Stream? GetResource(string lang)
        {
            return Assembly.GetExecutingAssembly()
             .GetManifestResourceStream($"ComicsLocalizationLib.Resources.Flags.{lang}.png");
        }
    }
}
