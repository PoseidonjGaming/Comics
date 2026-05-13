using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsLib.Utilities;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FuzzierSharp.Extractor;
using ModernDownladComics.Services;
using SearchComicsLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModernDownladComics.Models.View
{
    public partial class ImportPageViewModel(IComicsBuilderService comicService,
        JdownloaderService jdownloaderService, IPathService pathService) : ObservableObject
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

        public event Func<Task<bool>>? ScanEvent;
        public event Func<DialogArgs, Task<bool>>? SearchDialogEvent;
        public event Action<Comic?>? PathEvent;

        public void Load()
        {
            Files.Clear();
            foreach (var file in Directory.GetFiles(pathService.ComicsDir)
                .OrderBy(File.GetLastWriteTimeUtc).Select(dir => Path.GetFileName(dir)))
            {
                Files.Add(file);
            }

            SelectedFile = Files.FirstOrDefault() ?? "";

        }

        [RelayCommand]
        public async Task AddComic()
        {
            if (ScanEvent == null || PathEvent == null) return;

            bool res = await ScanEvent.Invoke();
            Comic? comic = await comicService.MakeComics(Comic.BaseURL,
                Comic.Author, Comic.PackageName, Comic.NumberPages, res);
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
        public async Task SearchComic()
        {
            if (SelectedFile == null || SearchDialogEvent == null) return;
            if (string.IsNullOrWhiteSpace(Comic.Author) || 
                string.IsNullOrWhiteSpace(Comic.PackageName))
                return;

            string? jd = await jdownloaderService.GetComicJdownloader(Comic.Author,
               Comic.PackageName);
            bool res = await SearchDialogEvent.Invoke(new(Comic.PackageName, Comic.Author,
                pathService.BackupDirPath, jd));
            if (res)
            {
                await AddComic();
            }
        }
    }
}
