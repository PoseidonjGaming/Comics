using ComicsLib.Models;
using ComicsLib.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Models;
using ModernDownladComics.Models.View;
using ModernDownladComics.Services;
using ModernDownladComics.Utilities;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPage : Page
    {
        public AddPageViewModel ViewModel { get; set; }
        public AddPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetRequiredService<AddPageViewModel>();
            ViewModel.NavigateEvent += comic =>
            {
                Frame.Navigate(typeof(PathPage), new PathPageArgs(comic, typeof(AddPage)));
            };
            ViewModel.AddDialogEvent += async () =>
            {
                ContentDialog dialog = new()
                {
                    Title = "Scan the url",
                    Content = "Do you want to scan the url",
                    DefaultButton = ContentDialogButton.Primary,
                    PrimaryButtonText = "Yes",
                    SecondaryButtonText = "No",
                    XamlRoot = this.XamlRoot
                };

                var res = await dialog.ShowAsync();
                return res == ContentDialogResult.Primary;
            };
            ViewModel.SearchDialogEvent += async (args) =>
            {
                string manga = ContentPageUtility.Search(args.Name, args.Author,
                    FileUtility.ComicsDirectory, "Manga");
                string backup = ContentPageUtility.Search(args.Name, args.Author,
                    args.Path, "Backup");

                ContentDialog dialog = new()
                {
                    Title = "Search",
                    Content = new SearchPage(manga, backup, !string.IsNullOrEmpty(args.Jd)?
                    args.Jd : "Download not found",
                    $"Do you want to add {args.Name}"),
                    PrimaryButtonText = "Yes",
                    DefaultButton = ContentDialogButton.Primary,
                    CloseButtonText = "No",
                    XamlRoot = this.XamlRoot
                };
                var res = await dialog.ShowAsync();
                return res == ContentDialogResult.Primary;
            };

            DataContext = ViewModel;
        }
    }
}
