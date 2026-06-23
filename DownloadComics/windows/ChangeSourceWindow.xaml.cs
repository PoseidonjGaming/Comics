using System.Windows;
using System.Windows.Controls;
using ComicsInfraLib.Models.Views;
using ComicsLib.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DownloadComics.windows
{
    /// <summary>
    /// Logique d'interaction pour ChangeSourceWindow.xaml
    /// </summary>
    public partial class ChangeSourceWindow : Window
    {
        public ChangeSourcePageViewModel ViewModel { get; set; }
        public ChangeSourceWindow(Comic comic)
        {
            InitializeComponent();
            ViewModel = App.Current.ServiceProvider.GetRequiredService<ChangeSourcePageViewModel>();
            ViewModel.Init(comic);
            DataContext = ViewModel;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.ChangeSource();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
