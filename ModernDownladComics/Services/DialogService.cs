using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Pages;
using ModernDownladComics.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernDownladComics.Services
{
    public class DialogService : IDialogService<XamlRoot>
    {
        public async Task<DialogResult> ShowAddAsync(XamlRoot xamlRoot)
        {
            Dictionary<string, string> data = App.Current.LocalizationService.GetData("ScanDialog", "Dialog");
            ContentDialog dialog = new()
            {
                Title = data["Title"],
                Content = data["Content"],
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = data["Yes"],
                SecondaryButtonText = data["No"],
                CloseButtonText = data["Cancel"],
                XamlRoot = xamlRoot
            };

            var res = await dialog.ShowAsync();
            return GetResult(res);
        }

        public async Task<DialogResult> ShowSearchAsync(DialogArgs args, XamlRoot xamlRoot)
        {
            Dictionary<string, string> data = App.Current.LocalizationService.GetData("SearchPage", "Dialog");
            string manga = ContentPageUtility.Search(args.Name, args.Author,
                FileUtility.ComicsDirectory, "Manga");
            string backup = ContentPageUtility.Search(args.Name, args.Author,
                args.Path, "Backup");

            string from = data["From"];
            string add = data["Add"];

            ContentDialog dialog = new()
            {
                Title = data["Search"],
                Content = new SearchPage(manga, backup, !string.IsNullOrEmpty(args.Jd) ?
                $"{from} JD: {args.Jd}" : data["NotFound"],
                $"{add} {args.Name}"),
                PrimaryButtonText = data["Yes"],
                DefaultButton = ContentDialogButton.Primary,
                CloseButtonText = data["No"],
                XamlRoot = xamlRoot
            };
            var res = await dialog.ShowAsync();
            return GetResult(res);
        }

        public async Task ShowErrorAsync(XamlRoot xamlRoot, string msg)
        {
            Dictionary<string, string> data = App.Current.LocalizationService.GetData("Dialog");
            ContentDialog dialog = new()
            {
                Title = data["Error"],
                Content = msg,
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = data["Close"],
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
            Dictionary<string, string> loc = App.Current.LocalizationService.GetData("Dialog");
            ContentDialog dialog = new()
            {
                Title = loc["Restore"],
                Content = loc["RestoreQuestion"],
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = loc["Yes"],
                CloseButtonText = loc["No"],
                XamlRoot = arg
            };

            var res = await dialog.ShowAsync();
            return GetResult(res);
        }
    }
}
