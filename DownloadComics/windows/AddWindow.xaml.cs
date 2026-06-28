using System.Windows;
using ComicsInfraLib.Models.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using DownloadComics.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DownloadComics.windows
{
    /// <summary>
    /// Logique d'interaction pour AddWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class AddWindow : Window
    {
        public AddPageViewModel<Window> ViewModel { get; set; }

        [ObservableProperty]
        public partial string AddBTNContent { get; set; }
        [ObservableProperty]
        public partial string SearchBTNContent { get; set; }
        [ObservableProperty]
        public partial string DeleteBTNContent { get; set; }
        [ObservableProperty]
        public partial string ClearBTNContent { get; set; }

        private const string Comics = "Entities.Comics";
        public AddWindow()
        {
            InitializeComponent();
            ViewModel = App.Current.ServiceProvider
                .GetRequiredService<AddPageViewModel<Window>>();
            ViewModel.NavigateEvent += (comic) =>
            {
                PathWindow pathWindow = new(comic)
                {
                    Owner = this
                };
                pathWindow.ShowDialog();
                ViewModel.Comic.Reset();
            };
            DataContext = ViewModel;

            var localizationService = App.Current.ServiceProvider.GetRequiredService<DownloadLocalizationService>();
            AddBTNContent = string.Format(localizationService["Buttons.AddBTN"],
                localizationService[Comics]);
            SearchBTNContent = string.Format(localizationService["Buttons.SearchBTN"],
                localizationService[Comics]);
            DeleteBTNContent = string.Format(localizationService["Buttons.DeleteBTN"],
                localizationService[Comics]);
            ClearBTNContent = string.Format(localizationService["Buttons.ClearBTN"],
                localizationService[Comics]);
        }
    }
}
