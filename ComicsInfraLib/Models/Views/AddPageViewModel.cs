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
    public partial class AddPageViewModel<T>: ObservableObject where T : class
    {

        private readonly IComicsBuilderService builderService;
        private readonly JdownloaderService jdownloaderService;
        private readonly IPathService pathService;
        private readonly IStateRepository stateRepository;
        private readonly IDialogService<T> dialogService;

        [ObservableProperty]
        public partial ComicInputModel Comic { get; set; } = new();
        [ObservableProperty]
        public partial Comic SelectedComic { get; set; } = new Comic();
        public ObservableCollection<Comic> Comics { get; set; }

        public event Action<Comic>? NavigateEvent;

        public AddPageViewModel(IComicsBuilderService builderService,
        JdownloaderService jdownloaderService, IPathService pathService,
        IStateRepository stateRepository, IDialogService<T> dialogService,
        LocalizationService localizationService)
        {
            this.builderService = builderService;
            this.jdownloaderService = jdownloaderService;
            this.pathService = pathService;
            this.stateRepository = stateRepository;
            this.dialogService = dialogService;

            Comics = new(stateRepository.Comics);
        }


        [RelayCommand]
        public async Task AddComic(T arg)
        {
            if (Comic.Author == null)
                Comic.Author = "";

            DialogResult res = await dialogService.ShowAddAsync(arg);
            if (res == DialogResult.CANCELLED)
                return;

            Comic? comic = await builderService.MakeComics(Comic.BaseURL,
                Comic.Author, Comic.PackageName, Comic.NumberPages, res == DialogResult.SUCCESS);

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
                pathService.BackupDirPath, jd), arg);
            if (res == DialogResult.SUCCESS)
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
        public void DelecteSelectComic()
        {
            stateRepository.Comics.Remove(SelectedComic);
            stateRepository.Save();
        }
    }
}
