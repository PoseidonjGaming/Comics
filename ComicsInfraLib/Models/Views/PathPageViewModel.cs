using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views
{
    public partial class PathPageViewModel : ObservableObject
    {
        private Comic Comic { get; set; }
        public ObservableCollection<string> Roots { get; }
        [ObservableProperty]
        public partial string SelectedRoot { get; set; }
        public ObservableCollection<string> Paths { get; }
        [ObservableProperty]
        public partial string SelectedPath { get; set; }

        private readonly IPathService _pathService;
        private readonly IScanService _scanService;
        private readonly IStateRepository _stateRepository;
        private CancellationTokenSource? _scanCts;
        public Type? ReturnType;

        public event Action<Type>? NavigateEvent;

        public PathPageViewModel(ISettingsService settingsService, IPathService pathService,
            IScanService scanService, IStateRepository stateRepository)
        {
            _pathService = pathService;
            _scanService = scanService;
            _stateRepository = stateRepository;
            Roots = new ObservableCollection<string>(settingsService.GetOptions().Paths);
            SelectedRoot = Roots.FirstOrDefault() ?? "";
            Paths = [];
            SelectedPath = "";
            Comic = new();
        }

        public void Init(PathPageArgs args)
        {
            if (args.Comic != null)
                Comic = args.Comic;
            ReturnType = args.ReturnType;
        }

        [RelayCommand]
        public void Select()
        {
            if (ReturnType == null || NavigateEvent == null) return;
            Comic.Path = Path.Combine(SelectedRoot, SelectedPath.Trim());

            try
            {
                if (Directory.Exists(Comic.Path))
                {
                    string destPath = Comic.Path.Replace(SelectedRoot,
                        _pathService.BackupDirPath);

                    DirectoryInfo? destInfo = Directory.GetParent(destPath);

                    if (destInfo is not null)
                    {
                        if (!destInfo.Exists)
                        {
                            destInfo.Create();
                        }
                    }

                    if (_pathService.BackupDirPath[0] == SelectedRoot[0])
                    {
                        _pathService.MoveComic(destPath, Comic.Path);
                    }
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            finally
            {
                _stateRepository.Comics.Add(Comic);
                _stateRepository.Tracks.Add(new(Comic.BaseURL, Comic.URL, Comic.Host));
                _stateRepository.Save();
               

                if (ReturnType != null)
                    NavigateEvent(ReturnType);
            }
        }
        [RelayCommand]
        public void Cancel()
        {
            if (ReturnType == null || NavigateEvent == null) return;
            NavigateEvent(ReturnType);
        }

        public async Task Load(Func<bool> getPreviousState, Action<bool> setState)
        {
            _scanCts = new CancellationTokenSource();
            bool previousIsEnabled = getPreviousState();
            try
            {
                setState(false);
                var results = await _scanService.ScanAsync(Comic, Roots, _scanCts.Token);
                foreach (var res in results)
                {
                    Paths.Add(res);
                }


            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                setState(previousIsEnabled);
            }
        }

        public void Unload()
        {
            if (_scanCts != null && !_scanCts.IsCancellationRequested)
            {
                _scanCts.Cancel();
            }
        }
    }
}