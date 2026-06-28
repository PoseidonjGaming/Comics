using ComicsInfraLib.Models.Views;
using ComicsServiceLib.UI;
using DownloadComics.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DownloadComics.windows
{
    /// <summary>
    /// Logique d'interaction pour SendWindow.xaml
    /// </summary>
    public partial class SendWindow : Window
    {
        public SendViewModel ViewModel { get; set; }
        public SendWindow()
        {
            InitializeComponent();
            ViewModel = App.Current.ServiceProvider
                .GetRequiredService<SendViewModel>();
            DataContext = ViewModel;

            JobState? jobState = App.Current.ServiceProvider.GetRequiredService<IJobState>() as JobState;
            jobState?.Init(ViewModel);
        }

        private async void JobToggleBTN_Checked(object sender, RoutedEventArgs e)
        {
            await ViewModel.StartJobAsync(() => { DialogResult = true; });
        }

        private void JobToggleBTN_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel.Cancel();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
           await ViewModel.Load(() => DialogResult = true);
        }
    }
}
