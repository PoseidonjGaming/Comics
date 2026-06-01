using ComicsLib.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ComicsInfraLib.Models.Views.Settings
{
    public partial class SettingsComicPageViewModel(Comic? comic): ObservableObject
    {
        [ObservableProperty]
        public partial Comic? Comic { get; set; } = comic;
        public Array Priorities { get; set; } = Enum.GetValues<Priorities>();

        public void Setup(SettingsPageArgs<Comic> args)
        {
            Comic = args.Arg;
        }
    }
}
