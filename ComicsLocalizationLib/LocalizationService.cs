using ComicsLocalizationLib.Resources;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ComicsLocalizationLib
{
    public partial class LocalizationService : INotifyPropertyChanged
    {
        protected Dictionary<string, string> _data = [];
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

        public virtual string Get(string key)
        {
            return _data.TryGetValue(key, out var value) ? value : $"#{key}#";
        }

        public void LoadLang(string lang)
        {
            string fileResource = GetRessource($"{lang}.json", "ComicsLocalizationLib.Resources.Langs");

            Dictionary<string, string> baseData = ReadLang(fileResource);
            IEnumerable<Dictionary<string, string>> layers = LoadAdditionnalLayers(lang);
            _data = Merge(baseData, layers);

            CurrentCulture = lang;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }

        private Dictionary<string, string> Merge(Dictionary<string, string> baseData, IEnumerable<Dictionary<string, string>> layers)
        {
            Dictionary<string,string> result= new Dictionary<string,string>(baseData);
            foreach (Dictionary<string, string> layer in layers)
            {
                foreach (var item in layer)
                {
                    result[item.Key] = item.Value;
                }
            }

            return result;
        }

        protected virtual IEnumerable<Dictionary<string, string>> LoadAdditionnalLayers(string lang)
        {
            yield break;
        }

        protected static Dictionary<string, string> ReadLang(string fileResource)
        {
            using Stream? stream = AppDomain.CurrentDomain.GetAssemblies()
                .First(assembly => assembly.GetName().Name == fileResource.Split(".").First())
                .GetManifestResourceStream(fileResource)
                ?? throw new Exception("The stream is empty");

            using StreamReader reader = new(stream);
            string json = reader.ReadToEnd();

            return Deserializer(json);
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

        protected static string GetRessource(string lang, string path)
        {

            return AppDomain.CurrentDomain.GetAssemblies()
                .First(assembly => assembly.GetName().Name == path.Split(".").First())
                 .GetManifestResourceNames().First(s =>
                 s.Equals($"{path}.{lang}"));
        }

        public static Stream? GetResource(string lang)
        {
            return Assembly.GetExecutingAssembly()
             .GetManifestResourceStream($"ComicsLocalizationLib.Resources.Flags.{lang}.png");
        }

        public void Notify()
        {

        }
    }
}
