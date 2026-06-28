using ComicsInfraLib.Models;
using ComicsInfraLib.Models.Views.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ModernDownladComics.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsAppPage : Page
    {
        public SettingsAppPageViewModel<WindowId> ViewModel { get; set; }
        public WindowId WindowId { get; set; }

        public SettingsInputModel? InputModel { get; set; }

        public SettingsAppPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetRequiredService<SettingsAppPageViewModel<WindowId>>();
            ViewModel.Init();

            ViewModel.PathChanged += path => InputModel?.Path = path;
            ViewModel.LangChanged += lang => InputModel?.Lang = lang;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is SettingsPageArgs<SettingsInputModel> args)
            {
                InputModel = args.Arg;
            }
        }

        private void Lang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.LoadLang();
        }
    }
}
