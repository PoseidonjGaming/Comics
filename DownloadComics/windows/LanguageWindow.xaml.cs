using DownloadComics.resources;
using DownloadComics.resources.flags;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DownloadComics.resources.language;

namespace DownloadComics.windows
{
    /// <summary>
    /// Logique d'interaction pour LanguageWindow.xaml
    /// </summary>
    public partial class LanguageWindow : Window
    {
        private readonly ObservableCollection<LanguageOption> Languages = [
            new("fr", "French"),
            new("en", "English"),
            ];

        public LanguageWindow()
        {
            InitializeComponent();

            langCMB.ItemsSource = Languages;
            langCMB.SelectedIndex = 0;
        }

        private void SelectLangBTN_Click(object sender, RoutedEventArgs e)
        {
            if (langCMB.SelectedItem is LanguageOption lang) { 
                Properties.Settings.Default.Lang = lang.Code;
                Properties.Settings.Default.Save();

                MessageBox.Show(LanguageStrings.Msg_Language_Saved);

                DialogResult = true;
            }
        }

        private void LangCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (langCMB.SelectedItem is LanguageOption lang) { 
                TranslationSource.Instance.CurrentCulture = new(lang.Code);
            }
        }
    }
}
