using ComicsInfraLib.Models;
using ComicsInfraLib.Models.Views;
using ComicsLib.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Utilities;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ImportPage : Page
{
    public ImportPageViewModel<XamlRoot> ViewModel { get; set; }

    public ImportPage()
    {
        InitializeComponent();
        ViewModel = App.Current.Services.GetRequiredService<ImportPageViewModel<XamlRoot>>();
        ViewModel.PathEvent += comic =>
        {
            Frame.Navigate(typeof(PathPage), new PathPageArgs(comic, typeof(ImportPage)));
        };

        App.Current.LocalizationService.LanguageChangedEvent += (data) =>
        {
            ViewModel.Loc = App.Current.LocalizationService.GetData("ImportPage", "Comic");
            Bindings.Update();
        };
        ViewModel.InitData(App.Current.LocalizationService.GetData("ImportPage", "Comic"));
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Load();
    }
}
