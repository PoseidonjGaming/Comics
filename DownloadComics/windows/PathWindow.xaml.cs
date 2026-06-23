using ComicsInfraLib.Models;
using ComicsInfraLib.Models.Views;
using ComicsLib.Models;
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
    /// Logique d'interaction pour PathWindow.xaml
    /// </summary>
    public partial class PathWindow : Window
    {
        public PathPageViewModel ViewModel { get; set; }
        public PathWindow(Comic comic)
        {
            InitializeComponent();

            ViewModel = App.Current.ServiceProvider.GetRequiredService<PathPageViewModel>();
            ViewModel.Init(new(comic, typeof(MainWindow)));
            ViewModel.NavigateEvent += type => DialogResult = true;
            DataContext = ViewModel;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.Load(() => IsEnabled, state => IsEnabled = state);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Unload();
        }
    }
}
