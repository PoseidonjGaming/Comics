using ComicsInfraLib.Models.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.windows;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; set; }
        public MainPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetRequiredService<MainPageViewModel>();
            ViewModel.ChangeSourceRequested += async comic =>
            {
                _ = new ChangeSourceWindow(comic);
            };

            App.Current.LocalizationService.LanguageChangedEvent += (data) =>
            {
                ViewModel.Loc = App.Current.LocalizationService.GetData("MainPage", "Comic");
                Bindings.Update();
            };
            ViewModel.Init(App.Current.LocalizationService.GetData("MainPage", "Comic"));
        }

        private void FilterTXT_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.FilterComics();
        }

        private void Hosts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.FilterComics();
        }
    }
}
