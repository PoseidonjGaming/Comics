using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FuzzierSharp.Extractor;
using SearchComicsLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ModernDownladComics.Models.View
{
    public partial class AddPageViewModel(IComicsBuilderService builderService,
        JdownloaderService jdownloaderService, IPathService pathService) : ObservableObject
    {
        [ObservableProperty]
        public partial Comic Comic { get; set; } = new();
        [ObservableProperty]
        public partial Comic SelectedComic { get; set; } = new Comic();
        public ObservableCollection<Comic> Comics { get; set; } = AppStateStore.Instance.Comics;

        public event Func<Task<bool>>? AddDialogEvent;
        public event Func<string, string, string?, Task<bool>>? SearchDialogEvent;
        public event Action<Comic>? NavigateEvent;

        [RelayCommand]
        public async Task AddComic()
        {
            if (AddDialogEvent == null || NavigateEvent == null) return;

            bool res = await AddDialogEvent.Invoke();
            Comic? comic = await builderService.MakeComics(Comic.BaseURL,
                Comic.Author.Trim(), Comic.PackageName.Trim(), Comic.NumberPages, res);

            if (comic == null) return;
            NavigateEvent.Invoke(comic);
        }

        [RelayCommand]
        public async Task SearchComic()
        {
            if (SearchDialogEvent == null || NavigateEvent == null) return;

            string? jd = await jdownloaderService.GetComicJdownloader(Comic.Author, Comic.PackageName);

            bool res = await SearchDialogEvent.Invoke(Comic.PackageName, 
                pathService.BackupDirPath, jd);
            if (res)
            {
                await AddComic();
            }
        }

        public string AddToPanel(string path, string from)
        {
            string authorPath = SearchUtility.GetAuthorPath(Comic.Author, path);

            IEnumerable<ExtractedResult<string>> results = SearchUtility.GetComics(authorPath,
                Comic.PackageName);
            ExtractedResult<string>? res = results.FirstOrDefault();

            if (res == null)
                return "Comic not found";

            string fromPath = res.Value.Replace(authorPath, string.Empty)[1..];

            return $"From {from}: {SearchUtility.CountPage(res.Value)} pages - {fromPath}";
        }

        [RelayCommand]
        public void ClearComic()
        {
            Comic.Reset();
        }

        [RelayCommand]
        public void DelecteSelectComic()
        {
            AppStateStore.Instance.Comics.Remove(SelectedComic);

                FileUtility.WriteFile(pathService.TrackFilePath,
                    AppStateStore.Instance.Tracks);
                FileUtility.WriteFile(pathService.BackupFilePath,
                    AppStateStore.Instance.Comics);
        }
    }
}
