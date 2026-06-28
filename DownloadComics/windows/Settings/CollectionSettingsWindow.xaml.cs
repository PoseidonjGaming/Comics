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
        public SettingsHostPageViewModel<Window> ViewModel { get; set; }

        [ObservableProperty]
        public partial string AddBTNContent { get; set; }
        [ObservableProperty]
        public partial string DeleteBTNContent { get; set; }
        public CollectionSettingsWindow(SettingsCollectionPageArg<Window> arg, string title)
        {
            InitializeComponent();
            ViewModel = App.Current.ServiceProvider
                .GetRequiredService<SettingsHostPageViewModel<Window>>();
            ViewModel.Setup(arg);
            DataContext = ViewModel;
            var localization = App.Current.ServiceProvider.GetRequiredService<ILocalizationService>();
            AddBTNContent = string.Format(localization.Get("Buttons.AddBTN"),
                localization.Get($"Entities.{arg.Entity}"));
            DeleteBTNContent = string.Format(localization.Get("Buttons.DeleteBTN"),
                localization.Get($"Entities.{arg.Entity}"));
            Title = string.Format(localization.Get("SettingsHostsWindow.Title"),
                localization.Get($"SettingsHostsWindow.{title}"));
        }
    }
}
