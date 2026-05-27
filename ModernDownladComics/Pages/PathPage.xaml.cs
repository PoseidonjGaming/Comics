using ComicsInfraLib.Models;
using ComicsInfraLib.Models.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PathPage : Page
    {

        public PathPageViewModel ViewModel { get; set; }


        public PathPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetRequiredService<PathPageViewModel>();
            ViewModel.NavigateEvent += (returnType) =>
            {
                Frame.Navigate(returnType);
            };
            App.Current.LocalizationService.LanguageChangedEvent += (data) =>
            {
                ViewModel.Loc = App.Current.LocalizationService.GetData("PathPage");
                Bindings.Update();
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is PathPageArgs args)
            {
                ViewModel.Init(args, App.Current.LocalizationService.GetData("PathPage"));
            }
        }

        private async void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
           await ViewModel.Load(() => IsEnabled, state => IsEnabled = state);
        }



        private void Page_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.Unload();
        }
    }
}
