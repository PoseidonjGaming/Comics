using ComicsLib.Models;
using ComicsLocalizationLib;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ModernDownladComics.Models
{
    public partial class AddLinksModelView(ILocalizationService localizationService) : ObservableObject
    {
        private const string Comics = "Entities.Comics";

        [ObservableProperty]
        public partial string AddBTNContent { get; set; } = 
            string.Format(localizationService.Get("Buttons.AddBTN"),
                localizationService.Get(Comics));
        [ObservableProperty]
        public partial string SearchBTNContent { get; set; } = 
            string.Format(localizationService.Get("Buttons.SearchBTN"),
                localizationService.Get(Comics));
        [ObservableProperty]
        public partial string DeleteBTNContent { get; set; } = 
            string.Format(localizationService.Get("Buttons.DeleteBTN"),
                localizationService.Get(Comics));
        [ObservableProperty]
        public partial string ClearBTNContent { get; set; } = 
            string.Format(localizationService.Get("Buttons.ClearBTN"),
                localizationService.Get(Comics));
    }
}
