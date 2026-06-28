using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsLocalizationLib;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views
{
    public partial class AddPageViewModel<T>(IComicsBuilderService builderService,
    JdownloaderService jdownloaderService, IPathService pathService,
    IStateRepository stateRepository, IDialogService<T> dialogService) : 
        ObservableObject where T : class
    {
        [ObservableProperty]
        public partial ComicInputModel Comic { get; set; } = new();
        [ObservableProperty]
        public partial Comic SelectedComic { get; set; } = new Comic();
        public ObservableCollection<Comic> Comics { get; set; } = stateRepository.Comics;

        public event Action<Comic>? NavigateEvent;

        [RelayCommand]
        public async Task AddComic(T arg)
        {
            if (Comic.Author == null)
                Comic.Author = "";

            DialogResult res = await dialogService.ShowAddAsync(arg);
            if (res == DialogResult.CANCELLED)
                return;

            Comic? comic = await builderService.MakeComics(Comic.BaseURL,
                Comic.Author, Comic.PackageName, Comic.NumberPages, res == DialogResult.YES);

            if (comic == null) return;
            NavigateEvent?.Invoke(comic);
        }

        [RelayCommand]
        public async Task SearchComic(T arg)
        {
            if (NavigateEvent == null) return;

            string? jd = await jdownloaderService.GetComicJdownloader(Comic.Author,
                Comic.PackageName);

            DialogResult res = await dialogService.ShowSearchAsync(new(Comic.PackageName, Comic.Author,
                pathService.BackupDirPath, jd), arg, "SearchDialog.Title", "SearchDialog.Content");
            if (res == DialogResult.YES)
            {
                await AddComic(arg);
            }
        }

        [RelayCommand]
        public void ClearComic()
        {
            Comic.Reset();
        }

        [RelayCommand]
        public void DeleteSelectComic()
        {
            Comics.Remove(SelectedComic);
            stateRepository.Save();
        }
    }
}
