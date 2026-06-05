using ComicsLocalizationLib;
using ComicsServiceLib.UI;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Threading.Tasks;

namespace ModernDownladComics.Services
{
    public class PickerDialog(LocalizationService localizationService) : IPickerDialog<WindowId>
    {

        public async Task<string> FileOpenDialog(WindowId arg, string title)
        {
            FileOpenPicker filePicker = new(arg)
            {
                Title = localizationService[title],
                CommitButtonText = localizationService["FileDialog.Select"]
            };

            filePicker.FileTypeChoices.Add("JSON", [".json"]);
            var res = await filePicker.PickSingleFileAsync();
            return res != null ? res.Path : string.Empty;
        }

        public async Task<string> FolderDialog(WindowId arg, string title)
        {
            FolderPicker dialog = new(arg)
            {
                Title = localizationService[title],
                CommitButtonText = localizationService["FolderDialog.Select"],
            };

            var res = await dialog.PickSingleFolderAsync();
            return res != null ? res.Path : string.Empty;
        }
    }
}
