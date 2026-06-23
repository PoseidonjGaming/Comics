using ComicsInfraLib.Models.Views.Settings;
using ComicsLib.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DownloadComics.Models
{
    public partial class DownloadSettingsComicsViewModel(Comic? comic) : SettingsComicPageViewModel(comic)
    {
        [ObservableProperty]
        public partial string EnableState { get; set; }
    }
}
