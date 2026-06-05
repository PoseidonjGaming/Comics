using ComicsLib.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ModernDownladComics.Models
{
    public partial class SettingsInputModel(Options options) : ObservableObject
    {
        public ObservableCollection<string> Hosts { get; set; } = new(options.Hosts);
        public ObservableCollection<string> Confirms { get; set; } = new(options.Confirms);
        public ObservableCollection<string> ExcludedHosts { get; set; } = new(options.ExcludedHosts);
       
        public ObservableCollection<string> Paths = new(options.Paths);

        [ObservableProperty]
        public partial string Path { get; set; } = options.Path;

        [ObservableProperty]
        public partial Comic? Comic { get; set; } = options.Comic;

        [ObservableProperty]
        public partial string Lang { get; set; } = options.Lang;
    }
}
