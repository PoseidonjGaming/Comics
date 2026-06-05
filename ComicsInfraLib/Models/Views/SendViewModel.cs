using ComicsInfraLib.Services;
using ComicsLib.Utility;
using ComicsLocalizationLib;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views
{
    public partial class SendViewModel(IStateRepository stateRepository,
        LocalizationService localizationService, JDownloadJobService jDownloadJobService,
        IPathService pathService) : ObservableObject
    {
        [ObservableProperty]
        public partial string Job { get; set; } = localizationService["SendPage.Start"];
        [ObservableProperty]
        public partial string StateJob { get; set; } = localizationService["SendPage.JobNotStarted"];
        [ObservableProperty]
        public partial int ProgressValue { get; set; }
        [ObservableProperty]
        public partial bool IsDetermined { get; set; } = true;
        [ObservableProperty]
        public partial bool IsChecked { get; set; }

        public int Maximum { get; set; } = stateRepository.Comics.Count;

        public ObservableCollection<string> States { get; } = [];

        private CancellationTokenSource? _verifyTokenSource;



        public async Task StartJobAsync(Action frameAction)
        {
            if (_verifyTokenSource == null || _verifyTokenSource.IsCancellationRequested)
            {
                _verifyTokenSource?.Dispose();
                _verifyTokenSource = new CancellationTokenSource();

                IsDetermined = false;

                IsChecked = true;
                Job = localizationService["SendPage.Stop"];

                await jDownloadJobService.RunAsync(_verifyTokenSource.Token);
                
                IsChecked = false;
                Job = localizationService["SendPage.Start"];

                frameAction();
            }
        }

        public void Cancel()
        {
            _verifyTokenSource?.Cancel();
            Job = localizationService["SendPage.Start"];
            IsChecked = false;
            IsDetermined = true;
            ProgressValue = 0;
        }

        public async Task Load(Action frameAction) {
            _verifyTokenSource = new CancellationTokenSource();
            IsDetermined = false;
            IsChecked = true;
            Job = localizationService["SendPage.Stop"];

            await jDownloadJobService.RunAsync(_verifyTokenSource.Token);
            stateRepository.Comics.Clear();
            stateRepository.Tracks.Clear();
            IsDetermined = true;
            FileUtility.WriteFile(pathService.BackupFilePath, stateRepository.Comics);
            frameAction();
        }
    }
}
