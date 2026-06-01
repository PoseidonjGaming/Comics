using ComicsLocalizationLib;
using ComicsLocalizationLib.Resources;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views.Settings
{
    public partial class SettingsAppPageViewModel<T>(IPickerDialog<T> pickerDialog,
        LocalizationService localizationService) : ObservableObject
    {
        public SettingsInputModel? Options { get; set; }

        [ObservableProperty]
        public partial LanguageOption Lang { get; set; }

        public ObservableCollection<LanguageOption> Languages { get; set; } =
            new(localizationService.Languages);

        [RelayCommand]
        public async Task SetPath(T arg)
        {
            Options?.Path = await pickerDialog.FolderDialog(arg, 
                localizationService["FileDialog.Title"]);
        }

        public void Init(SettingsInputModel inputModel)
        {
            Options = inputModel;
            Lang = Languages.First(l => l.Code == Options?.Lang);
        }

        public void LoadLang()
        {
            localizationService.LoadLang(Lang.Code);
            Options?.Lang = Lang.Code;
        }
    }
}
