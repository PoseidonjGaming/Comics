using ComicsInfraLib.Models;
using ComicsInfraLib.Models.Views;
using ComicsLib.Models;
using ComicsLocalizationLib;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Models;
using ModernDownladComics.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPage : Page
    {
        public AddPageViewModel<XamlRoot> ViewModel { get; set; }
        public AddLinksModelView AddLinksModelView { get; set; }

        public AddPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services
                .GetRequiredService<AddPageViewModel<XamlRoot>>();
            ViewModel.NavigateEvent += comic =>
            {
                Frame.Navigate(typeof(PathPage), new PathPageArgs(comic, typeof(AddPage)));
            };

            AddLinksModelView = App.Current.Services.GetRequiredService<AddLinksModelView>();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            i += 10;
        }
    }
}
