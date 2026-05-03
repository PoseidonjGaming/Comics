using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace ModernDownloadComics.Resources
{
    public class TranslationSource : INotifyPropertyChanged
    {
        private static readonly TranslationSource _instance = new();
        public static TranslationSource Instance => _instance;

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Dictionary<string, string> _localizations = [];
        private ResourceManager[] _managers = [];

        private CultureInfo _currentCulture = CultureInfo.GetCultureInfo("en");
        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                if (!_currentCulture.Equals(value))
                {
                    _currentCulture = value;

                    Thread.CurrentThread.CurrentCulture = value;
                    Thread.CurrentThread.CurrentUICulture = value;

                   
                }
                Reload();
                RaiseLanguageChanged();
            }
        }

        public string this[string key] => string.IsNullOrEmpty(key) ? string.Empty :
            (_localizations.TryGetValue(key, out string? value) && !string.IsNullOrEmpty(value) ? value : $"[{key}]");


        public void Load(string cultureName, params ResourceManager[] managers)
        {
            _managers = managers;
            CurrentCulture = new(cultureName);
        }

        public void Reload()
        {
           _localizations.Clear();

            foreach (ResourceManager manager in _managers)
            {
                ResourceSet? resourceSet = manager.GetResourceSet(_currentCulture, true, true);
                if (resourceSet != null) {
                    foreach (DictionaryEntry entry in resourceSet)
                    {
                        string? key = entry.Key.ToString();
                        if (!string.IsNullOrEmpty(key))
                        {
                            _localizations[key] = entry.Value?.ToString()?? $"[{key}]";
                        }

                    }
                }
            }
        }

        private void RaiseLanguageChanged()
        {
            PropertyChanged?.Invoke(this, new(string.Empty));
        }

    }
}
