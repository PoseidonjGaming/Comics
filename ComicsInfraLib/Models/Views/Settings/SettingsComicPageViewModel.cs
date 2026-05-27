using ComicsLib.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ComicsInfraLib.Models.Views
{
    public partial class SettingsComicPageViewModel : BaseLocViewModel
    {
        [ObservableProperty]
        public partial Comic Comic { get; set; } = new();
        public Array Priorities { get; set; } = Enum.GetValues<Priorities>();

        public void Setup(SettingsPageArgs<Comic> args)
        {
            Comic = args.Arg;
        }
    }
}
