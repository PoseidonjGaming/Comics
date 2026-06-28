using System.ComponentModel;
using ComicsLocalizationLib;

namespace DownloadComics.Services
{
    public class DownloadLocalizationService : LocalizationService, ILocalizationService, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void LoadLang(string lang)
        {
            string filename = $"{lang}.json";
            string baseFileResource = GetRessource(filename,
                "ComicsLocalizationLib.Resources.Langs");
            Dictionary<string, string> baseData = ReadLang(baseFileResource);

            string downloadFileResource = GetRessource(filename,
                "DownloadComics.Resources.Langs");
            Dictionary<string, string> downloadData = ReadLang(downloadFileResource);

            _data = baseData;
            foreach (var item in downloadData)
            {
                _data[item.Key] = item.Value;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }
    }
}
