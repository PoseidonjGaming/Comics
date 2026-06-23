using ComicsLocalizationLib;
using ComicsServiceLib.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace DownloadComics.Services
{
    public class PickerDialogService(DownloadLocalizationService localizationService,
        IPathService pathService) : IPickerDialog<Window>
    {
        public Task<string> FileOpenDialog(Window arg, string title)
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = string.Format(localizationService[title],
                localizationService["Dialog.File"]),
                AddExtension = true,
                DefaultExt = "json",
                Filter = "Json Files (.json) | *.json"
            };
            openFileDialog.ShowDialog(arg);
            return Task.FromResult(openFileDialog.FileName);
        }

        public Task<string> FolderOpenDialog(Window arg, string title)
        {
            OpenFolderDialog openFolderDialog = new()
            {
                Title = string.Format(localizationService[title], 
                localizationService["Dialog.Folder"]),
            };
            openFolderDialog.ShowDialog(arg);
            return Task.FromResult(openFolderDialog.FolderName);
        }

        public Task<string> SaveFileDialog(Window arg, string title)
        {
            SaveFileDialog saveFile = new()
            {
                InitialDirectory = pathService.BackupDirPath,
                AddExtension = true,
                DefaultExt = "json",
                Filter = $"Json Files (.json) | *.json"
            };
            saveFile.ShowDialog(arg);
            return Task.FromResult(saveFile.FileName);
        }
    }
}
