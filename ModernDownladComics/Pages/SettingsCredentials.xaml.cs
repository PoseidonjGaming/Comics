using ComicsInfraLib.Models;
using ComicsInfraLib.Models.Views;
using ComicsLib.Models;
using JDownloader;
using JDownloader.Model;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsCredentials : Page
    {
        public SettingsCredientialsPageViewModel ViewModel { get; set; }
        public SettingsCredentials()
        {
            InitializeComponent();
            ViewModel = new();
            ViewModel.ConnectionEvent += () =>
            {
                connectionLBL.Foreground = new SolidColorBrush(Colors.Green);
            };
            ViewModel.DialogEvent += async (msg) =>
            {
                connectionLBL.Foreground = new SolidColorBrush(Colors.Red);
                ContentDialog dialog = new()
                {
                    Title = "Error",
                    Content = msg,
                    DefaultButton = ContentDialogButton.Primary,
                    PrimaryButtonText = "Close",
                    XamlRoot = this.XamlRoot
                };

                await dialog.ShowAsync();
            };
            DataContext = ViewModel;
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is SettingsPageArgs<JDCredentials> args)
            {
                ViewModel.Setup(args);
            }
        }

       
    }
}
