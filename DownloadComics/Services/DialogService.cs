using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsLocalizationLib;
using ComicsServiceLib.UI;
using DownloadComics.windows.Dialogs;
using System.Windows;

namespace DownloadComics.Services
{
    public class DialogService(DownloadLocalizationService localizationService,
        ContentPageService<DownloadLocalizationService> contentPageService,
        ISettingsService settingsService) : IDialogService<Window>
    {
        public async Task<DialogResult> ShowAddAsync(Window arg)
        {
            AddDialogWindow addDialog = new(arg, localizationService["ScanDialog.Title"],
                localizationService["ScanDialog.Content"]);

            addDialog.ShowDialog();
            return await addDialog.Result.Task;
        }

        public Task ShowErrorAsync(Window arg, string msg)
        {
            MessageBox.Show(msg);
            return Task.CompletedTask;
        }

        public async Task<DialogResult> ShowRestoreAsync(Window arg, string comic)
        {
            RestoreDialogWindow restoreDialog = new(localizationService["RestoreDialog.Content"], comic)
            {
                Owner = arg
            };
            restoreDialog.ShowDialog();
            return await restoreDialog.Result.Task;
        }

        public async Task<DialogResult> ShowSearchAsync(DialogArgs args, Window arg, string title,
            string question)
        {
            string manga = contentPageService.Search(args.Name, args.Author,
                settingsService.GetOptions().Path, "Manga");
            string backup = contentPageService.Search(args.Name, args.Author,
                args.Path, "Backup");
            string jd = args.Jd ?? localizationService["SearchDialog.NoComment"];

            string from = localizationService["SearchDialog.From"];
            string windowTitle = string.Format(localizationService[title], args.Name);
            string windowQuestion = string.Format(localizationService[question], args.Name);

            SearchDialogWindow searchDialog = new(arg, windowTitle, windowQuestion, manga, backup, 
                $"{from} JD: {jd}");
            searchDialog.ShowDialog();
            return await searchDialog.Result.Task;
        }
    }
}
