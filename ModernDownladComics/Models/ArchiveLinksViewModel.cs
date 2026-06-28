using ComicsLocalizationLib;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ModernDownladComics.Models
{
    public partial class ArchiveLinksViewModel(ILocalizationService localizationService) :
        ObservableObject
    {

        [ObservableProperty]
        public partial string SearchBTNContent { get; set; } = 
            string.Format(localizationService.Get("Buttons.DeleteBTN"),
                localizationService.Get("Entities.Archive"));
        [ObservableProperty]
        public partial string DeleteBTNContent { get; set; } =
            string.Format(localizationService.Get("Buttons.SearchBTN"),
                localizationService.Get("Entities.Archive"));
    }
}
