using DownloadComics.models;
using DownloadComics.utilities;
using FuzzierSharp;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Windows;

namespace DownloadComics.windows
{
    /// <summary>
    /// Logique d'interaction pour ChangeSourceWindow.xaml
    /// </summary>
    public partial class ChangeSourceWindow : Window
    {
        private Comic comic;
        public ChangeSourceWindow(Comic comic)
        {
            InitializeComponent();
            this.comic = comic;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Options? options = JsonConvert.DeserializeObject<Options>(Properties.Settings.Default.Options ?? string.Empty);

            if (options != null)
            {
                sourcesCMB.ItemsSource = options.Hosts;
                sourcesCMB.SelectedItem = Process.ExtractOne(comic.URL, options.Hosts).Value;
            }
        }

        private void ChangeSourceBTN_Click(object sender, RoutedEventArgs e)
        {
            if (sourcesCMB.SelectedItem != null)
            {
                HtmlNode? baseNode = ComicUtility.GetBodyNode(comic.BaseURL, comic.HtmlBody, out string? body);
                if (baseNode != null)
                {
                    comic.URL = ComicUtility.GetUrlByHost(baseNode, sourcesCMB.Text);
                    DialogResult = true;
                }

            }
            else
            {
                DialogResult = false;
            }

        }


    }
}
