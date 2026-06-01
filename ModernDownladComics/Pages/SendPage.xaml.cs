using ComicsInfraLib.Models.Views;
using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Services;
using System;
using System.Threading;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SendPage : Page
    {

        private CancellationTokenSource? _verifyTokenSource;

        private readonly JDownloadJobService? _jobService;
        public readonly SendViewModel ViewModel;
        private readonly IStateRepository _stateRepository;

        public SendPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetRequiredService<SendViewModel>();

            _jobService = App.Current.Services.GetRequiredService<JDownloadJobService>();
            var service = App.Current.Services.GetRequiredService<IJobState>() as JobState;

            service?.InitPage(ViewModel);

            _stateRepository = App.Current.Services.GetRequiredService<IStateRepository>();
        }

        private async void JobToggleBTN_Checked(object sender, RoutedEventArgs e)
        {
            if (_verifyTokenSource == null || _verifyTokenSource.IsCancellationRequested)
            {
                _verifyTokenSource?.Dispose();
                _verifyTokenSource = new CancellationTokenSource();

                ViewModel.IsDetermined = false;

                jobToggleBTN.IsChecked = true;
                jobToggleBTN.Content = "Stop";

                if (_jobService != null)
                {
                    await _jobService.RunAsync(_verifyTokenSource.Token);
                    jobToggleBTN.IsChecked = false;
                    Frame.Navigate(typeof(MainPage));
                }

            }
        }

        private void jobToggleBTN_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _verifyTokenSource = new CancellationTokenSource();
            ViewModel.IsDetermined = false;
            jobToggleBTN.IsChecked = true;
            jobToggleBTN.Content = "Stop";

            if (_jobService != null)
            {
                await _jobService.RunAsync(_verifyTokenSource.Token);
                _stateRepository.Comics.Clear();
                ViewModel.IsDetermined = true;
                var pathService = App.Current.Services.GetRequiredService<IPathService>();
                FileUtility.WriteFile(pathService.BackupFilePath, _stateRepository.Comics);
                Frame.Navigate(typeof(MainPage));
            }


        }
    }
}
