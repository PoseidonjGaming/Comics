using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsLib.Services;
using ComicsServiceLib.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Services;
using System;
using System.Collections.ObjectModel;
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
        public ObservableCollection<string> States { get; set; } = [];

        private CancellationTokenSource? _verifyTokenSource;

        private readonly JDownloadJobService? _jobService;

        public SendPage()
        {
            InitializeComponent();
            jobToggleBTN.Content = "Start";
            tryTXT.Text = "Job not started";

            _jobService = App.Services?.GetRequiredService<JDownloadJobService>();
            var service = App.Services?.GetRequiredService<IJobState>() as JobState;
            service?.InitPage(this, tryTXT, progressbar);
        }

        private async void JobToggleBTN_Checked(object sender, RoutedEventArgs e)
        {
            if (_verifyTokenSource == null || _verifyTokenSource.IsCancellationRequested)
            {
                _verifyTokenSource?.Dispose();
                _verifyTokenSource = new CancellationTokenSource();

                progressbar.IsIndeterminate = true;

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
            progressbar.IsIndeterminate = true;
            jobToggleBTN.IsChecked = true;
            jobToggleBTN.Content = "Stop";

            if (_jobService != null)
            {
                await _jobService.RunAsync(_verifyTokenSource.Token);
                jobToggleBTN.IsChecked = false;
                FileService.WriteFile(FileService.BackupFilePath,
                    AppStateStore.Instance.Comics);
                Frame.Navigate(typeof(MainPage));
            }


        }
    }
}
