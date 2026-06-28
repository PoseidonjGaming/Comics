using System.Collections.Generic;
using System.ComponentModel;
using ComicsLocalizationLib;

namespace ModernDownladComics.Services
{
    public partial class ModernDownloadComicsLocalizationService :
        LocalizationService, ILocalizationService, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void LoadLang(string lang)
        {
            string filename = $"{lang}.json";
            string baseFileResource = GetRessource(filename,
                "ComicsLocalizationLib.Resources.Langs");
            Dictionary<string, string> baseData = ReadLang(baseFileResource);
            _data = baseData;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }
    }
}
