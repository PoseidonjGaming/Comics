using ComicsLib.Models;
using ComicsLocalizationLib;
using ComicsServiceLib.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Pages;
using ModernDownladComics.Utilities;
using System;
using System.Threading.Tasks;

namespace ModernDownladComics.Services
{
    public class DialogService(LocalizationService localizationService,
        ISettingsService settingsService) : IDialogService<XamlRoot>
    {
        public async Task<DialogResult> ShowAddAsync(XamlRoot xamlRoot)
        {
            ContentDialog dialog = new()
            {
                Title = localizationService["ScanDialog.Title"],
                Content = localizationService["ScanDialog.Content"],
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = localizationService["Dialog.Yes"],
                SecondaryButtonText = localizationService["Dialog.No"],
                CloseButtonText = localizationService["Dialog.Cancel"],
                XamlRoot = xamlRoot
            };

            var res = await dialog.ShowAsync();
            return GetResult(res);
        }

        public async Task<DialogResult> ShowSearchAsync(DialogArgs args, XamlRoot xamlRoot)
        {
            string manga = ContentPageUtility.Search(args.Name, args.Author,
                settingsService.GetOptions().Path, "Manga");
            string backup = ContentPageUtility.Search(args.Name, args.Author,
                args.Path, "Backup");

            string from = localizationService["SearchPage.From"];
            string add = localizationService["SearchPage.Add"];

            ContentDialog dialog = new()
            {
                Title = localizationService["SearchPage.Title"],
                Content = new SearchPage(manga, backup, !string.IsNullOrEmpty(args.Jd) ?
                $"{from} JD: {args.Jd}" : localizationService["SearchPage.NotFound"],
                $"{string.Format(add, args.Name)}"),
                PrimaryButtonText = localizationService["Dialog.Yes"],
                DefaultButton = ContentDialogButton.Primary,
                CloseButtonText = localizationService["Dialog.No"],
                XamlRoot = xamlRoot
            };
            var res = await dialog.ShowAsync();
            return GetResult(res);
        }

        public async Task ShowErrorAsync(XamlRoot xamlRoot, string msg)
        {
            ContentDialog dialog = new()
            {
                Title = localizationService["Dialog.Error"],
                Content = msg,
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = localizationService["Dialog.Close"],
                XamlRoot = xamlRoot
            };

            await dialog.ShowAsync();
        }
        private static DialogResult GetResult(ContentDialogResult res)
        {
            return res switch
            {
                ContentDialogResult.Primary => DialogResult.SUCCESS,
                ContentDialogResult.Secondary => DialogResult.FAILED,
                _ => DialogResult.CANCELLED
            };
        }

        public async Task<DialogResult> ShowRestoreAsync(XamlRoot arg)
        {
            ContentDialog dialog = new()
            {
                Title = localizationService["Dialog.Restore"],
                Content = localizationService["Dialog.RestoreQuestion"],
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = localizationService["Dialog.Yes"],
                CloseButtonText = localizationService["Dialog.No"],
                XamlRoot = arg
            };

            var res = await dialog.ShowAsync();
            return GetResult(res);
        }
    }
}
