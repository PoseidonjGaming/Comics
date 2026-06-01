using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsLib.Utilities;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views
{
    public partial class ImportPageViewModel<T>(IComicsBuilderService comicService,
        JdownloaderService jdownloaderService, IPathService pathService,
        IDialogService<T> dialogService): ObservableObject where T : class
    {
        public ObservableCollection<string> URLS { get; } = [];
        public ObservableCollection<string> Files { get; } = [];

        [ObservableProperty]
        public partial Comic Comic { get; set; } = new Comic();

        private string _selectedFile = "";
        public string SelectedFile
        {
            get => _selectedFile;
            set
            {
                if (SetProperty(ref _selectedFile, value))
                {
                    FileChanged();
                }
            }
        }

        public event Action<Comic?>? PathEvent;

        public void Load()
        {
            if (!Files.Any())
            {
                foreach (var file in Directory.GetFiles(pathService.ComicsDir)
                                .OrderBy(File.GetLastWriteTimeUtc).Select(dir => Path.GetFileName(dir)))
                {
                    Files.Add(file);
                }

                SelectedFile = Files.FirstOrDefault() ?? "";
            }
        }

        [RelayCommand]
        public async Task AddComic(T arg)
        {
            if (PathEvent == null) return;

            DialogResult res = await dialogService.ShowAddAsync(arg);
            if(res == DialogResult.CANCELLED) return;

            Comic? comic = await comicService.MakeComics(Comic.BaseURL,
                Comic.Author, Comic.PackageName, Comic.NumberPages, res == DialogResult.SUCCESS);
            if (comic != null)
                PathEvent.Invoke(comic);

            Comic.Reset();
        }

        [RelayCommand]
        public void ResetComic()
        {
            Comic.Reset();
        }

        public void FileChanged()
        {
            if (!string.IsNullOrEmpty(SelectedFile))
            {
                string path = Path.Combine(pathService.ComicsDir, SelectedFile);
                if (File.Exists(path))
                {
                    URLS.Clear();
                    JsonUtility.GetURLS(File.ReadAllText(path)).ForEach(URLS.Add);
                }

            }

        }

        [RelayCommand]
        public async Task SearchComic(T arg)
        {
            if (SelectedFile == null) return;
            if (string.IsNullOrWhiteSpace(Comic.Author) || 
                string.IsNullOrWhiteSpace(Comic.PackageName))
                return;

            string? jd = await jdownloaderService.GetComicJdownloader(Comic.Author,
               Comic.PackageName);
            DialogResult res = await dialogService.ShowSearchAsync(new(Comic.PackageName, Comic.Author,
                pathService.BackupDirPath, jd), arg);
            if (res == DialogResult.SUCCESS)
            {
                await AddComic(arg);
            }
        }
    }
}
