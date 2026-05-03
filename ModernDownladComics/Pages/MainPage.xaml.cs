using ComicsLib.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.windows;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Array Priorities { get; set; }
        public ObservableCollection<Comic> Comics { get; set; }
        public MainPage()
        {
            InitializeComponent();
            Priorities = Enum.GetValues<Priority>();
            Comics = new(AppStateStore.Instance.Comics);
        }

        private void changeSourceBTN_Click(object sender, RoutedEventArgs e)
        {
            if (comicsLST.SelectedItem is Comic comic)
            {
                ChangeSourceWindow changeSourceWindow = new(comic);
            }
                
        }

        private void UnselectBTN_Click(object sender, RoutedEventArgs e)
        {
            comicsLST.SelectedIndex = -1;
        }
    }
}
