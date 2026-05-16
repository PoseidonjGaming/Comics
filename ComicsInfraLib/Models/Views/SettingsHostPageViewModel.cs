using ComicsInfraLib.Helpers;
using ComicsLib.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views
{
    public partial class SettingsHostPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial ObservableCollection<string> Collection { get; set; }

        [ObservableProperty]
        public partial string Value { get; set; }
        [ObservableProperty]
        public partial string SelectedHost { get; set; }

        private bool isHost;

        public SettingsHostPageViewModel()
        {
            Value = string.Empty;
            Collection = [];
        }

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
