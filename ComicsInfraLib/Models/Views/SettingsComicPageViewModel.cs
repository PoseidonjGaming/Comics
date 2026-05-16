using ComicsLib.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ComicsInfraLib.Models.Views
{
    public partial class SettingsComicPageViewModel: ObservableObject
    {
        [ObservableProperty]
        public partial Comic Comic { get; set; }
        public Array Priorities { get; set; }

        public SettingsComicPageViewModel()
        {
            Comic = new();
            Priorities = Enum.GetValues<Priorities>();
        }

        public void Setup(SettingsPageArgs<Comic> args)
        {
            Comic = args.Arg;
        }
    }
}
