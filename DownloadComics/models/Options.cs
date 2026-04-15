using DownloadComics.resources.settings;
using System.Windows.Controls;

namespace DownloadComics.models
{
    public class Options
    {
        public static readonly string[] OptionList = [
            SettingsStrings.Settings_Host_Search,
            SettingsStrings.Settings_Host_Confirm,
            SettingsStrings.Settings_Path,
            SettingsStrings.Settings_Excluded_Host];
        public Comic? Comic { get; set; }
        public string[] Hosts { get; set; } = [];
        public string[] Confirms { get; set; } = [];
        public string[] Paths { get; set; } = [];
        public string[] ExcludedHosts { get; set; } = [];
        public sbyte Period { get; set; } = 0;
        private string Lang { get; set; } = "en";

        public Options(Comic comic, string[] hosts, string[] confirms, string[] paths, string[] excludedHosts, sbyte period, string lang)
        {
            Comic = comic;
            Hosts = hosts;
            Confirms = confirms;
            Paths = paths;
            Period = period;
            ExcludedHosts = excludedHosts;
            Lang = lang;
        }


        public Options() { }

        public static void AddToCombo(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            foreach (var option in OptionList)
            {
                comboBox.Items.Add(option);
            }
        }

    }
}
