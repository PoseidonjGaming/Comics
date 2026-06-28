using ComicsInfraLib.Helpers;
using ComicsLocalizationLib;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views.Settings
{
    public partial class SettingsHostPageViewModel<T>(ILocalizationService localizationService,
        IPickerDialog<T> pickerDialog) : ObservableObject
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

        public T? Arg { get; set; }

        public void Setup(SettingsCollectionPageArg<T> settings)
        {
            Collection = settings.List;
            isHost = settings.IsHost;
            Arg = settings.Arg;

            Label = string.Format("{0}:", localizationService.Get(isHost ? "Comic.Host" : "Comic.Path"));
        }

        [RelayCommand]
        public async Task AddValue(T arg)
        {
            if (isHost)
            {
                Collection.Add(RegexUtility.HostRegex().Match(Value).Value);
            }
            else
            {
                string path = await pickerDialog.FolderOpenDialog(arg, "SaveFileDialog.Title");
                Collection.Add(path);
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
