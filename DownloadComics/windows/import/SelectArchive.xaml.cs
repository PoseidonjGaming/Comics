using DownloadComics.services;
using System.IO;
using System.Windows;

namespace DownloadComics.windows
{
    /// <summary>
    /// Logique d'interaction pour ChooseArchive.xaml
    /// </summary>
    public partial class SelectArchive : Window
    {
        public string? SelectedFile { get; private set; }

        public SelectArchive()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fileCB.Items.Clear();
           
            if (!Directory.Exists(FileService.ComicsDir))
                return;

            var files = Directory.GetFiles(FileService.ComicsDir, "*", SearchOption.AllDirectories)
                                 .OrderBy(File.GetLastWriteTimeUtc);

            foreach (var file in files)
            {
                fileCB.Items.Add(Path.GetFileName(file));
            }

            var waitedFile = files.Where(f=>Path.GetDirectoryName(f) == FileService.ComicsDir)
                                  .OrderBy(File.GetLastWriteTimeUtc)
                                  .Select(Path.GetFileName)
                                  .FirstOrDefault();

            fileCB.SelectedItem = waitedFile;
        }

        private void ImpotBtn_Click(object sender, RoutedEventArgs e)
        {
            if (fileCB.SelectedItem is string fileName && !string.IsNullOrWhiteSpace(fileName))
            {
                SelectedFile = fileName;
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }
            Close();
        }

        
    }
}
