using ComicsInfraLib.Helpers;
using ComicsInfraLib.Models;
using ComicsInfraLib.Models.Views;
using ComicsLib.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsHostsPage : Page
{
    public SettingsHostPageViewModel ViewModel { get; set; }
    public SettingsHostsPage()
    {
        InitializeComponent();
        ViewModel = new();
        DataContext = ViewModel;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if(e.Parameter is SettingsPageArgs<ObservableCollection<string>> settings)
        {
            ViewModel.Setup(settings);
        }
    }
}
