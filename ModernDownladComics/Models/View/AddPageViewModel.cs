using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ModernDownladComics.Models.View
{
    public partial class AddPageViewModel(IComicsBuilderService builderService,
        JdownloaderService jdownloaderService, IPathService pathService,
        IStateRepository stateRepository) : ObservableObject
    {
        [ObservableProperty]
        public partial ComicInputModel Comic { get; set; } = new();
        [ObservableProperty]
        public partial Comic SelectedComic { get; set; } = new Comic();
        public ObservableCollection<Comic> Comics { get; set; } = stateRepository.Comics;

        public event Func<Task<bool>>? AddDialogEvent;
        public event Func<DialogArgs, Task<bool>>? SearchDialogEvent;
        public event Action<Comic>? NavigateEvent;

        [RelayCommand]
        public async Task AddComic()
        {
            if (AddDialogEvent == null || NavigateEvent == null) return;

            bool res = await AddDialogEvent.Invoke();
            if (Comic.Author == null)
                Comic.Author = "";
            Comic? comic = await builderService.MakeComics(Comic.BaseURL,
                Comic.Author, Comic.PackageName, Comic.NumberPages, res);

            if (comic == null) return;
            NavigateEvent.Invoke(comic);
        }

        [RelayCommand]
        public async Task SearchComic()
        {
            if (SearchDialogEvent == null || NavigateEvent == null) return;

            string? jd = await jdownloaderService.GetComicJdownloader(Comic.Author, Comic.PackageName);

            bool res = await SearchDialogEvent.Invoke(new(Comic.PackageName, Comic.Author,
                pathService.BackupDirPath, jd));
            if (res)
            {
                await AddComic();
            }
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
            stateRepository.Save();
        }
    }
}
