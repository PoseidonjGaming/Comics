using DownloadComics.models;
using DownloadComics.resources.main;
using DownloadComics.services;
using DownloadComics.utilities;
using DownloadComics.windows.settings;
using FuzzierSharp;
using FuzzierSharp.PreProcess;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;



namespace DownloadComics.windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static AppState State => AppStateStore.Instance;

        private readonly ICollectionView CollectionView;
        public string Filter { get; set; } = "";

        public MainWindow()
        {
            InitializeComponent();

            CollectionView = CollectionViewSource.GetDefaultView(State.GetComics());

            CollectionView.Filter = o =>
            {
                if (o is not Comic comic)
                    return false;



                bool filterName = true;
                if (!string.IsNullOrEmpty(Filter))
                {
                    filterName = Fuzz.TokenSetRatio(Filter, $"{comic.PackageName} {comic.Author}",
                        StandardPreprocessors.CaseInsensitive) >= 90;
                }

                bool filterHost = true;
                string? selectedHost = filterHostCMB.SelectedItem as string;
                if (!string.IsNullOrEmpty(selectedHost) && selectedHost != "All")
                {
                    filterHost = comic.Host == selectedHost;
                }

                return filterName && filterHost;
            };


            comicsURLsList.ItemsSource = State.GetComics();

            InitFolder(FileService.BackupDirPath);
            InitFolder(FileService.ComicsDir);

            Options? options = JsonConvert.DeserializeObject<Options>(Properties.Settings.Default.Options);
            if (options == null)
            {
                Properties.Settings.Default.Options = JsonConvert.SerializeObject(new Options());
                Properties.Settings.Default.Save();
            }
                
            if (File.Exists(FileService.BackupFilePath))
            {
                string backup = File.ReadAllText(FileService.BackupFilePath);
                List<Comic>? comics = JsonConvert.DeserializeObject<List<Comic>>(backup);
                if (comics is not null)
                {
                    foreach (var c in comics)
                        State.GetComics().Add(c);
                }

                PopulateFilterCombo();
                if (File.Exists(FileService.TrackFilePath))
                {
                    State.Tracks = FileService.ReadFile<List<Track>>(FileService.TrackFilePath) ?? [];
                }
            }
        }

        private static void InitFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void WriteBackup()
        {
            FileService.WriteFile(FileService.BackupFilePath, State.GetComics());
        }
        private void ClearSelectedComic()
        {
            dUrlTXT.Clear();
            dAuthorTXT.Clear();
            filenameTXT.Clear();
            pkgNameTXT.Clear();
            pagesTXT.Clear();
            hostTXT.Clear();
            pathTXT.Clear();
            enabledCB.IsChecked = false;
            priorityCombo.SelectedIndex = 3;


            if (State.GetComics().Count > 0)
            {
                comicsURLsList.SelectedIndex = -1;
            }
        }

        private void ClearDownload()
        {
            urlTXT.Clear();
            authorTXT.Clear();
            comicNameTXT.Clear();
            numberPagesTXT.Text = "0";
        }

        private void PopulateFilterCombo()
        {
            filterHostCMB.Items.Clear();
            filterHostCMB.Items.Add("All");

            foreach (string host in State.GetComics().Select(c => c.Host).Distinct())
            {
                filterHostCMB.Items.Add(host);
            }
            filterHostCMB.SelectedIndex = 0;
        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(numberPagesTXT.Text, out int pages))
            {
                ComicUtility.MakeComic(urlTXT.Text, authorTXT.Text, comicNameTXT.Text, pages, this,
                    isScan.IsChecked.GetValueOrDefault());
                PopulateFilterCombo();

                FileService.WriteFile(FileService.TrackFilePath, State.Tracks);
                WriteBackup();

                ClearDownload();
            }

        }



        private void ComicsURLsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(comicsURLsList.SelectedItem is Comic comic)
            {
                enabledCB.IsChecked = comic.Enabled;
                priorityCombo.SelectedItem = comic.Priority;
            }
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (comicsURLsList.SelectedItem is Comic comic)
            {
                State.RemoveComic(comic);
                ClearSelectedComic();
                WriteBackup();
                PopulateFilterCombo();
            }
        }

        private void GenerateBTN_Click(object sender, RoutedEventArgs e)
        {
            VerifyWindow verifyWindow = new()
            {
                Owner = this,
            };

            verifyWindow.Closed += (sender, e) =>
            {
                ClearSelectedComic();
            };


            if (verifyWindow.ShowDialog() == true && File.Exists(FileService.BackupFilePath))
            {
                File.Delete(FileService.BackupFilePath);
            }
        }

        private void ClearPrevSession(object sender, RoutedEventArgs e)
        {
            if (File.Exists(FileService.BackupFilePath))
            {
                File.Delete(FileService.BackupFilePath);
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (comicsURLsList.SelectedItem is Comic comic)
            {
                Match hostMatch = RegexUtility.HostRegex().Match(dUrlTXT.Text);
                hostTXT.Text = hostMatch.Success ? hostMatch.Value : string.Empty;

                comic.URL = new(dUrlTXT.Text);
                comic.Author = dAuthorTXT.Text;
                comic.Filename = filenameTXT.Text;
                comic.PackageName = pkgNameTXT.Text;
                comic.Path = pathTXT.Text;
                comic.Host = hostTXT.Text;
                if (int.TryParse(pagesTXT.Text, out int pages))
                {
                    comic.NumberPages = pages;
                }
                comic.Enabled = enabledCB.IsChecked ?? false;
                if (Enum.TryParse<models.Priority>(priorityCombo.Text, out models.Priority priority))
                    comic.Priority = priority;

                ClearSelectedComic();
                
                WriteBackup();
            }
        }

        private void PagesTXT_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !RegexUtility.NumberRegex().IsMatch(e.Text);
        }

        private void PagesTXT_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = e.DataObject.GetData(typeof(string)) as string ?? string.Empty;
                if (!RegexUtility.NumberRegex().IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void UnselectBtn_Click(object sender, RoutedEventArgs e)
        {
            comicsURLsList.SelectedIndex = -1;
            ClearSelectedComic();

        }



        private void DUrlTXT_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void ImportItem(object sender, RoutedEventArgs e)
        {
            SelectArchive archive = new()
            {
                Owner = this
            };


            if (archive.ShowDialog() == true && !string.IsNullOrEmpty(archive.SelectedFile))
            {

                IEnumerable<string> dirs = Directory.EnumerateFiles(FileService.ComicsDir, "*", SearchOption.AllDirectories);

                var result = Process.ExtractOne(archive.SelectedFile, dirs, s => Path.GetFileName(s));
                if (result != null && result.Score == 100)
                {
                    try
                    {
                        ImportWindows importWindows = new(JsonUtility.GetURLS(File.ReadAllText(result.Value)))
                        {
                            Owner = this,
                            Title = $"{MainStrings.Main_Import_Title} {archive.SelectedFile}"
                        };

                        importWindows.Closed += (sender, e) =>
                        {
                            CultureInfo cultureInfo = new("en-EN");

                            Match match = RegexUtility.DateRegex().Match(Path.GetFileNameWithoutExtension(result.Value));
                            if (match.Success && DateTime.TryParse(match.Value, out DateTime date))
                            {
                                string monthName = cultureInfo.DateTimeFormat.GetMonthName(date.Month);
                                string year = date.Year.ToString();

                                string toMovePath = Path.Combine(FileService.ComicsDir,
                                    year, monthName, archive.SelectedFile);

                                if (!Directory.Exists(Path.Combine(FileService.ComicsDir, year, monthName)))
                                {
                                    Directory.CreateDirectory(Path.Combine(FileService.ComicsDir, year, monthName));
                                }

                                File.Move(result.Value, toMovePath);
                            }
                        };
                        importWindows.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{MainStrings.Msg_Import_Error} : {ex.Message}", MainStrings.Msg_Error_Tile,
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show(MainStrings.Msg_File_Not_Found, MainStrings.Msg_Error_Tile,
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void RestoreBackup(object sender, RoutedEventArgs e)
        {
            RestoreBackup windows = new()
            {
                Owner = this
            };
            windows.ShowDialog();
        }



        private void SettingsItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new()
            {
                Owner = this
            };

            settings.ShowDialog();
        }

        private void SearchBTN_Click(object sender, RoutedEventArgs e)
        {
            ComicUtility.SearchComic(authorTXT.Text, comicNameTXT.Text.Trim());
        }

        private void ExportSettingsItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new()
            {
                InitialDirectory = FileService.CurrentDir,
                AddExtension = true,
                DefaultExt = "json",
                Filter = $"{MainStrings.Main_File_Dialog_Json} (.json) | *.json"
            };

            if (saveFile.ShowDialog() == true)
            {

                Options? options = JsonConvert.DeserializeObject<Options>(Properties.Settings.Default.Options);
                File.WriteAllText(saveFile.FileName, JsonConvert.SerializeObject(options, Formatting.Indented));
            }
        }

        private void ImportSettings_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                InitialDirectory = FileService.CurrentDir,
                AddExtension = true,
                DefaultExt = "json",
                Filter = $"{MainStrings.Main_File_Dialog_Json} (.json) | *.json"
            };

            if (dialog.ShowDialog() == true)
            {
                Properties.Settings.Default.Options = File.ReadAllText(dialog.FileName);

                Properties.Settings.Default.Save();
                MessageBox.Show(MainStrings.Msg_Import_Settings, MainStrings.Msg_Import_Settings_Title,
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ChangeSourceBTN_Click(object sender, RoutedEventArgs e)
        {
            if (comicsURLsList.SelectedItem is Comic comic)
            {
                ChangeSourceWindow changeSourceWindow = new(comic)
                {
                    Owner = this
                };

                changeSourceWindow.Closed += (sender, e) =>
                {
                    if (comicsURLsList.SelectedItem is Comic comic)
                    {
                        dUrlTXT.Text = comic.URL;
                        dAuthorTXT.Text = comic.Author;
                        filenameTXT.Text = comic.Filename;
                        pkgNameTXT.Text = comic.PackageName;
                        pagesTXT.Text = comic.NumberPages.ToString();
                        hostTXT.Text = comic.Host;
                        pathTXT.Text = comic.Path;
                        enabledCB.IsChecked = comic.Enabled;
                        priorityCombo.Text = comic.Priority.ToString();

                        ImportWindows? importWindow = Application.Current.Windows.OfType<ImportWindows>().FirstOrDefault();

                        importWindow?.Navigate(comic.BaseURL);
                    }
                };
                changeSourceWindow.ShowDialog();
            }
        }

        private void OpenJDownloaderSettingsClick(object sender, RoutedEventArgs e)
        {
            CredentialsSettings credentials = new()
            {
                Owner = this
            };
            credentials.ShowDialog();
        }

        private void ChangeLang_Click(object sender, RoutedEventArgs e)
        {
            LanguageWindow languageWindow = new()
            {
                Owner = this
            };

            languageWindow.ShowDialog();
        }


        private void FilterTXT_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionView.Refresh();
        }

        private void ClearComicsBTN_Click(object sender, RoutedEventArgs e)
        {
            ClearDownload();
        }

        private void FilterHostCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionView.Refresh();
        }
    }
}