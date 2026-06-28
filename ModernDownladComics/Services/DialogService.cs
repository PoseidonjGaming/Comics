using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsLocalizationLib;
using ComicsServiceLib.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Pages;
using System;
using System.Threading.Tasks;

namespace ModernDownladComics.Services
{
    public class DialogService(ILocalizationService localizationService,
        ISettingsService settingsService, ContentPageService contentPageService) : 
        IDialogService<XamlRoot>
    {
        public async Task<DialogResult> ShowAddAsync(XamlRoot xamlRoot)
        {
            ContentDialog dialog = new()
            {
                Title = localizationService.Get("ScanDialog.Title"),
                Content = localizationService.Get("ScanDialog.Content"),
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = localizationService.Get("Dialog.Yes"),
                SecondaryButtonText = localizationService.Get("Dialog.No"),
                CloseButtonText = localizationService.Get("Buttons.CancelBTN"),
                XamlRoot = xamlRoot
            };

            var res = await dialog.ShowAsync();
            return GetResult(res);
        }

        public async Task<DialogResult> ShowSearchAsync(DialogArgs args, XamlRoot xamlRoot, string title, string content)
        {
            string manga = contentPageService.Search(args.Name, args.Author,
                settingsService.GetOptions().Path, "Manga");
            string backup = contentPageService.Search(args.Name, args.Author,
                args.Path, "Backup");
            string jd = args.Jd ?? localizationService.Get("SearchDialog.NoComment");

            string from = localizationService.Get("SearchDialog.From");

            string windowTitle = string.Format(localizationService.Get(title), args.Name);
            string windowQuestion = string.Format(localizationService.Get(content), args.Name);

            ContentDialog dialog = new()
            {
                Title = windowTitle,
                Content = new SearchPage(manga, backup, $"{from} JD: {jd}", windowQuestion),
                PrimaryButtonText = localizationService.Get("Dialog.Yes"),
                DefaultButton = ContentDialogButton.Primary,
                CloseButtonText = localizationService.Get("Dialog.No"),
                XamlRoot = xamlRoot
            };
            var res = await dialog.ShowAsync();
            return GetResult(res);
        }

        public async Task ShowErrorAsync(XamlRoot xamlRoot, string msg)
        {
            ContentDialog dialog = new()
            {
                Title = localizationService.Get("Dialog.Error"),
                Content = msg,
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = localizationService.Get("Dialog.Close"),
                XamlRoot = xamlRoot
            };

            await dialog.ShowAsync();
        }
        private static DialogResult GetResult(ContentDialogResult res)
        {
            return res switch
            {
                ContentDialogResult.Primary => DialogResult.YES,
                ContentDialogResult.Secondary => DialogResult.NO,
                _ => DialogResult.CANCELLED
            };
        }

        public async Task<DialogResult> ShowRestoreAsync(XamlRoot arg, string title)
        {
            ContentDialog dialog = new()
            {
                Title = localizationService.Get("RestoreDialog.Title"),
                Content = localizationService.Get("RestoreDialog.Content"),
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = localizationService.Get("Dialog.Yes"),
                CloseButtonText = localizationService.Get("Dialog.No")  ,
                XamlRoot = arg
            };

            var res = await dialog.ShowAsync();
            return GetResult(res);
        }
    }
}
