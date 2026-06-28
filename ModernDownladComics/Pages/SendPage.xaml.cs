using ComicsInfraLib.Models.Views;
using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsLocalizationLib;
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

        public readonly SendViewModel ViewModel;

        public SendPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetRequiredService<SendViewModel>();

            var service = App.Current.Services.GetRequiredService<IJobState>() as JobState;

            service?.InitPage(ViewModel);

        }

        private async void JobToggleBTN_Checked(object sender, RoutedEventArgs e)
        {
            await ViewModel.StartJobAsync(() => Frame.Navigate(typeof(MainPage)));
        }

        private void JobToggleBTN_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel.Cancel();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.Load(() => Frame.Navigate(typeof(MainPage)));
        }
    }
}
