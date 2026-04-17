using DownloadComics.models;
using DownloadComics.resources.settings;
using DownloadComics.utilities;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;


namespace DownloadComics.windows.settings
{
    /// <summary>
    /// Logique d'interaction pour DefaultComics.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {

        private readonly List<OptionCollection> Collections = [];
        private Options? options;
        public SettingsWindow()
        {
            InitializeComponent();


            string optionsJson = Properties.Settings.Default.Options ?? string.Empty;
            foreach (string item in Options.OptionList)
            {
                Collections.Add(new OptionCollection(item));
            }
            if (!string.IsNullOrEmpty(optionsJson))
            {

                try
                {
                    options = JsonConvert.DeserializeObject<Options>(optionsJson);
                }
                catch (JsonException)
                {
                    options = new();
                }
            }
            else
            {
                options = new();
            }



            if (options != null)
            {
                if (options.Comic != null)
                    enabledCB.IsChecked = options.Comic.Enabled;

                AddToCollection(Collections[0], options.Hosts);
                AddToCollection(Collections[1], options.Confirms);
                AddToCollection(Collections[2], options.Paths);
                AddToCollection(Collections[3], options.ExcludedHosts);
            }

            hostList.ItemsSource = Collections[0].collections;
            settingCB.ItemsSource = Collections;
        }

        private static void AddToCollection(OptionCollection target, string[]? strings)
        {
            if (strings == null || strings.Length == 0)
                return;

            target.Feed(strings);
        }

        private static Comic CreateFallback()
        {
            return new Comic(
                "https://exemple_url.com",
                "https://exemple_baseurl.com",
                "package name",
                "filename",
                "host.com",
                0,
                "author");
        }

        private void UpdateBTN_Click(object sender, RoutedEventArgs e)
        {
            updataBTN.IsEnabled = false;

            options ??= new();
            options.Comic ??= CreateFallback();

            options.Comic.Enabled = enabledCB.IsChecked == true;

            options.Hosts = ToArraySafe(Collections.Count > 0 ? Collections[0].collections : null);
            options.Confirms = ToArraySafe(Collections.Count > 1 ? Collections[1].collections : null);
            options.Paths = ToArraySafe(Collections.Count > 2 ? Collections[2].collections : null);
            options.ExcludedHosts = ToArraySafe(Collections.Count > 3 ? Collections[3].collections : null);

            var optionsToSave = options;

            Task.Run(() =>
            {
                string json = JsonConvert.SerializeObject(optionsToSave, Formatting.Indented);

                Properties.Settings.Default.Options = json;
                Properties.Settings.Default.Save();

                Dispatcher.Invoke(() =>
                {
                    updataBTN.IsEnabled = true;
                    MessageBox.Show(SettingsStrings.Msg_Settings_Saved, SettingsStrings.Msg_Settings_Saved_Title,
                        MessageBoxButton.OK, MessageBoxImage.Information);
                });
            });
        }

        private static string[] ToArraySafe(ObservableCollection<string>? list)
        {
            if (list == null || list.Count == 0)
                return [];

            var result = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
                result[i] = list[i] ?? string.Empty;

            return result;
        }

        private void SettingCB_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (settingCB.SelectedItem is OptionCollection option)
            {
                hostList.ItemsSource = option.collections;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (options != null && options.Comic != null)
            {
                dUrlTXT.Text = options.Comic.URL;
                hostTXT.Text = options.Comic.Host;
                dAuthorTXT.Text = options.Comic.Author;
                pkgNameTXT.Text = options.Comic.PackageName;
                filenameTXT.Text = options.Comic.Filename;
                extansionTXT.Text = options.Comic.Extansion;
            }

        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var collection = hostList.ItemsSource as IList<string>;
            if (RegexUtility.HostRegex().IsMatch(itemTXT.Text) || Path.Exists(itemTXT.Text))
            {
                collection?.Add(itemTXT.Text);
                itemTXT.Clear();
            }
            else
            {
                MessageBox.Show(SettingsStrings.Msg_Host_Error);
            }

        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (settingCB.SelectedItem is OptionCollection optCollection)
            {
                if (hostList.SelectedIndex != -1)
                {
                    optCollection.collections.RemoveAt(hostList.SelectedIndex);
                }
            }
        }
    }
}
