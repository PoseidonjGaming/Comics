using ComicsInfraLib.Helpers;
using ComicsLocalizationLib;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views
{
    public partial class SettingsHostPageViewModel(LocalizationService localizationService) :
        ObservableObject
    {
        [ObservableProperty]
        public partial ObservableCollection<string> Collection { get; set; } = [];

        [ObservableProperty]
        public partial string Value { get; set; } = string.Empty;
        [ObservableProperty]
        public partial string SelectedHost { get; set; } = string.Empty;

        private bool isHost;

        [ObservableProperty]
        public partial string Label { get; set; }

        public void Setup(SettingsCollectionPageArg settings)
        {
            Collection = settings.List;
            isHost = settings.IsHost;

            Label = string.Format("{0}:", localizationService[isHost ? "Comic.Host" : "Comic.Path"]);
        }

        [RelayCommand]
        public void AddValue()
        {
            Collection.Add(isHost ? RegexUtility.HostRegex().Match(Value).Value : Value);

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
