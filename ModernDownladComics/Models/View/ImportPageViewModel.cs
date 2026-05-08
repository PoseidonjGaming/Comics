using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsLib.Utilities;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FuzzierSharp.Extractor;
using Microsoft.UI.Xaml.Controls.Primitives;
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
    public partial class ImportPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial ObservableCollection<string> URLS { get; set; }
        public ObservableCollection<string> Files { get; }

        [ObservableProperty]
        public partial Comic Comic { get; set; }

        [ObservableProperty]
        public partial string SelectedFile { get; set; }

        private readonly IComicsBuilderService _comicService;
        private readonly JdownloaderService _jdownloaderService;
        private readonly IPathService _pathService;

        public event Func<Task<bool>>? ScanEvent;
        public event Func<string?, IPathService, string, Task<bool>>? SearchDialogEvent;
        public event Action<Comic?>? PathEvent;

        public ImportPageViewModel(IComicsBuilderService comicService,
            JdownloaderService jdownloaderService, IPathService pathService)
        {
            URLS = [];
            Files = new ObservableCollection<string>(Directory.GetFiles(pathService.ComicsDir)
           .OrderBy(File.GetLastWriteTimeUtc).Select(dir => Path.GetFileName(dir)));
            Comic = new Comic();
            SelectedFile = Files.First();

            _comicService = comicService;
            _jdownloaderService = jdownloaderService;
            _pathService = pathService;
        }

        [RelayCommand]
        public async Task AddComic()
        {
            if (ScanEvent == null || PathEvent == null) return;

            bool res = await ScanEvent.Invoke();
            Comic? comic = await _comicService.MakeComics(Comic.BaseURL,
                Comic.Author, Comic.PackageName, Comic.NumberPages, res);

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
            string path = Path.Combine(_pathService.ComicsDir, SelectedFile);
            URLS = new ObservableCollection<string>(JsonUtility.GetURLS(File.ReadAllText(path)));
        }

        [RelayCommand]
        public async Task SearchComic()
        {
            if (SelectedFile == null || SearchDialogEvent == null) return;

            string? jd = await _jdownloaderService.GetComicJdownloader(Comic.Author,
               Comic.PackageName);
            bool res = await SearchDialogEvent.Invoke(jd, _pathService, Comic.PackageName);
            if (res)
            {
                await AddComic();
            }
        }

        public string AddToPanel(string path, string from)
        {
            if (Comic == null)
                return "null";
            string authorPath = SearchUtility.GetAuthorPath(Comic.Author, path);

            IEnumerable<ExtractedResult<string>> results = SearchUtility.GetComics(authorPath,
                Comic.PackageName);
            ExtractedResult<string>? res = results.FirstOrDefault();

            if (res == null)
                return "Comic not found";

            string fromPath = res.Value.Replace(authorPath, string.Empty)[1..];

            return $"From {from}: {SearchUtility.CountPage(res.Value)} pages - {fromPath}";
        }
    }
}
