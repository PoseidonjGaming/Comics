using ComicsInfraLib.Models;
using ComicsInfraLib.Models.Views.Settings;
using ComicsLocalizationLib;
using CommunityToolkit.Mvvm.ComponentModel;
using DownloadComics.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DownloadComics.windows.Settings
{
    /// <summary>
    /// Logique d'interaction pour CollectionSettingsWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class CollectionSettingsWindow : Window
    {
        public SettingsHostPageViewModel<Window, DownloadLocalizationService> ViewModel { get; set; }

        [ObservableProperty]
        public partial string AddBTNContent { get; set; }
        [ObservableProperty]
        public partial string DeleteBTNContent { get; set; }
        public CollectionSettingsWindow(SettingsCollectionPageArg arg, string entity, string title)
        {
            InitializeComponent();
            ViewModel = App.Current.ServiceProvider
                .GetRequiredService<SettingsHostPageViewModel<Window, DownloadLocalizationService>>();
            ViewModel.Setup(arg);
            DataContext = ViewModel;
            var localization = App.Current.ServiceProvider.GetRequiredService<DownloadLocalizationService>();
            AddBTNContent = string.Format(localization["Buttons.AddBTN"],
                localization[$"Entities.{entity}"]);
            DeleteBTNContent = string.Format(localization["Buttons.DeleteBTN"],
                localization[$"Entities.{entity}"]);
            Title = string.Format(localization["SettingsHostsWindow.Title"],
                localization[$"SettingsHostsWindow.{title}"]);
        }
    }
}
