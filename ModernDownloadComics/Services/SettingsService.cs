using System;
using Newtonsoft.Json;
using Windows.Storage;

namespace ModernDownloadComics.Services
{
    public static class SettingsService
    {
        private static ApplicationDataContainer LocalSettings => ApplicationData.Current.LocalSettings;

        /// <summary>
        /// Sauvegarde une valeur (tout type) dans LocalSettings (sérialisée en JSON).
        /// </summary>
        public static void Set<T>(string key, T value)
        {
            string json = JsonConvert.SerializeObject(value);
            LocalSettings.Values[key] = json;
        }

        /// <summary>
        /// Récupère une valeur. Retourne defaultValue si absente ou désérialisable.
        /// </summary>
        public static T? Get<T>(string key, T? defaultValue = default)
        {
            if (LocalSettings.Values.TryGetValue(key, out var obj) && obj is string s)
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(s) ?? defaultValue;
                }
                catch
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        public static bool Contains(string key) => LocalSettings.Values.ContainsKey(key);

        public static void Remove(string key) => LocalSettings.Values.Remove(key);
    }
}