using ComicsInfraLib.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views
{
    public partial class SettingsHostPageViewModel(Dictionary<string, string> loc) : ObservableObject
    {
        [ObservableProperty]
        public partial ObservableCollection<string> Collection { get; set; } = [];

        [ObservableProperty]
        public partial string Value { get; set; } = string.Empty;
        [ObservableProperty]
        public partial string SelectedHost { get; set; } = string.Empty;

        private bool isHost;

        public Dictionary<string, string> Loc { get; set; } = loc;

        public void Setup(SettingsPageArgs<ObservableCollection<string>> settings)
        {
            Collection = settings.Arg;
            isHost = settings.IsHost;
        }

        [RelayCommand]
        public void AddValue()
        {
            if (isHost)
            {
                Collection.Add(RegexUtility.HostRegex().Match(Value).Value);
            }
            else
            {
                Collection.Add(Value);
            }

            Value = string.Empty;
        }

        [RelayCommand]
        public void DeleteValue()
        {
            if (!string.IsNullOrEmpty(SelectedHost))
                Collection.Remove(SelectedHost);
        }
    }
}
