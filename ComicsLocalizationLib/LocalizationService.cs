using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ComicsLocalizationLib
{
    public class LocalizationService
    {
        private Dictionary<string, object> _data = [];
        public Dictionary<string, object> Data { get => _data; }
        public string CurrentCulture { get; private set; } = "en";

        public string this[string key] => Get(key);
        public event Action<Dictionary<string, object>>? LanguageChangedEvent;

        public string Get(string key)
        {
            var parts = key.Split('.');
            object current = _data;
            foreach (var part in parts)
            {
                if (current is Dictionary<string, object> dic &&
                    dic.TryGetValue(part, out var next))
                {
                    current = next;
                }
                else
                {
                    return $"#{key}#";
                }
            }

            return current.ToString() ?? $"#{key}#";
        }

        public string Format(string key, object value)
        {
            string template = Get(key);

            if (value == null)
                return template;

            var props = value.GetType().GetProperties();
            foreach (var prop in props)
            {
                string placeholder = "{" + prop.Name + "}";
                string val = prop.GetValue(value)?.ToString() ?? "";

                template = template.Replace(placeholder, val);
            }
            return template;
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

            LanguageChangedEvent?.Invoke(_data);

        }

        private static Dictionary<string, object> Deserializer(string json)
        {
            Dictionary<string, object> rootDic = [];

            JsonNode? root = JsonNode.Parse(json);
            if (root != null)
            {
                foreach (var rootProp in root.AsObject())
                {
                    if (rootProp.Value != null
                        && rootProp.Value.GetValueKind() == JsonValueKind.Object)
                    {
                        Dictionary<string, string> dic = [];
                        foreach (var prop in rootProp.Value.AsObject())
                        {
                            if (prop.Value != null && prop.Value.GetValueKind() == JsonValueKind.String)
                                dic.Add(prop.Key, prop.Value.AsValue().GetValue<string>());
                        }
                        rootDic.Add(rootProp.Key, dic);
                    }
                }
            }

            return rootDic;
        }

        private static string GetRessource(string lang)
        {

            var list = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            return Assembly.GetExecutingAssembly()
                 .GetManifestResourceNames().First(s => s.Equals($"ComicsLocalizationLib.{lang}"));
        }

        public Dictionary<string, string> GetData(string uiKey, params string[] additionalKeys)
        {
            Dictionary<string, string> dic = GetDic(uiKey);

            foreach (var key in additionalKeys)
            {
                Dictionary<string, string> commonDic = GetDic(key);
                foreach (var item in commonDic)
                {
                    dic.TryAdd(item.Key, item.Value);
                }
            }

            return dic;
        }

        private Dictionary<string, string> GetDic(string uiKey)
        {
            return _data[uiKey] as Dictionary<string, string> ?? [];
        }
    }
}
