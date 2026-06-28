using ComicsInfraLib.Models;
using ComicsInfraLib.Models.Views;
using ComicsInfraLib.Models.Views.Settings;
using ComicsLocalizationLib;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.WindowsAppSDK.Runtime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsHostsPage : Page
{
    public SettingsHostPageViewModel<WindowId> ViewModel { get; set; }

    public string AddBTNContent { get; set; }
    public string DeleteBTNContent { get; set; }
    public SettingsHostsPage()
    {
        InitializeComponent();
        ViewModel = App.Current.Services.GetRequiredService<SettingsHostPageViewModel<WindowId>>();
        
        AddBTNContent = string.Empty;
        DeleteBTNContent = string.Empty;

    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if(e.Parameter is SettingsCollectionPageArg<WindowId> settings)
        {
            ViewModel.Setup(settings);

            var localization = App.Current.Services.GetRequiredService<ILocalizationService>();
            AddBTNContent = string.Format(localization.Get("Buttons.AddBTN"),
            localization.Get($"Entities.{settings.Entity}"));
            DeleteBTNContent = string.Format(localization.Get("Buttons.DeleteBTN"),
                localization.Get($"Entities.{settings.Entity}"));
        }
    }
}
