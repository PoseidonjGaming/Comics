using ComicsLocalizationLib;
using ComicsServiceLib.UI;
using Microsoft.UI;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Threading.Tasks;

namespace ModernDownladComics.Services
{
    public class PickerDialog(ILocalizationService localizationService) : IPickerDialog<WindowId>
    {

        public async Task<string> FileOpenDialog(WindowId arg, string title)
        {
            FileOpenPicker filePicker = new(arg)
            {
                Title = string.Format(localizationService.Get(title),
                localizationService.Get("Dialog.File")),
                CommitButtonText = string.Format(localizationService.Get(title),
                localizationService.Get("Dialog.File"))
            };

            filePicker.FileTypeChoices.Add("JSON", [".json"]);
            var res = await filePicker.PickSingleFileAsync();
            return res != null ? res.Path : string.Empty;
        }

        public async Task<string> FolderOpenDialog(WindowId arg, string title)
        {
            FolderPicker dialog = new(arg)
            {
                Title = string.Format(localizationService.Get(title),
                localizationService.Get("Dialog.Folder")),
                CommitButtonText = string.Format(localizationService.Get(title),
                localizationService.Get("Dialog.Folder"))
            };

            var res = await dialog.PickSingleFolderAsync();
            return res != null ? res.Path : string.Empty;
        }

        public async Task<string> SaveFileDialog(WindowId arg, string title)
        {
            FileSavePicker filePicker = new(arg)
            {
                Title = localizationService.Get(title),
                CommitButtonText = localizationService.Get("SaveFileDialog.SaveBTN")
            };
            filePicker.FileTypeChoices.Add("JSON", [".json"]);
            var res = await filePicker.PickSaveFileAsync();
            return res != null ? res.Path : string.Empty;
        }
    }
}
