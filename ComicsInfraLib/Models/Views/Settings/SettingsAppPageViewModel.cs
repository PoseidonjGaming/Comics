using ComicsLib.Models;
using ComicsLocalizationLib;
using ComicsLocalizationLib.Resources;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views.Settings
{
    public partial class SettingsAppPageViewModel<T, L>(IPickerDialog<T> pickerDialog,
        L localizationService, ISettingsService settingsService) :
        ObservableObject where L : LocalizationService
    {
        [ObservableProperty]
        public partial string Path { get; set; }

        [ObservableProperty]
        public partial LanguageOption Lang { get; set; }

        public ObservableCollection<LanguageOption> Languages { get; set; } =
            new(localizationService.Languages);

        public event Action<string>? PathChanged;
        public event Action<string>? LangChanged;

        [RelayCommand]
        public async Task SetPath(T arg)
        {
            string path = await pickerDialog.FolderOpenDialog(arg, "FileDialog.Title");
            if (!string.IsNullOrEmpty(path))
                PathChanged?.Invoke(path);
        }

        public void Init()
        {
            Path = settingsService.GetOptions().Path;
            Lang = Languages.First(l => l.Code == settingsService.GetOptions().Lang);
        }

        public void LoadLang()
        {
            localizationService.LoadLang(Lang.Code);
            LangChanged?.Invoke(Lang.Code);
        }
    }
}
