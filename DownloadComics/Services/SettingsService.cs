using ComicsLib.Models;
using ComicsServiceLib.UI;
using Newtonsoft.Json;

namespace DownloadComics.Services
{
    public class SettingsService : ISettingsService
    {
        public Options Options;

        public SettingsService()
        {
            Options = JsonConvert.DeserializeObject<Options>(Properties.Settings.Default.Options) ?? new();
        }
        public Options GetOptions()
        {
            return Options;
        }

        public void SaveOptions()
        {
            Properties.Settings.Default.Options = JsonConvert.SerializeObject(Options);
            Properties.Settings.Default.Save();
        }

        public void SetOptions(Options options)
        {
            Options = options;
        }
    }
}
