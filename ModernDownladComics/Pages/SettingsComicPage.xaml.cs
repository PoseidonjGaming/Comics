using ComicsLib.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ModernDownladComics.Models;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsComicPage : Page
    {
        public Comic Comic { get; set; }
        public Array Priorities { get; set; }
        public SettingsComicPage()
        {
            InitializeComponent();
            Comic = new();
            Priorities = Enum.GetValues<Priority>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is SettingsPageArgs<Comic> args)
            {
                Comic = args.Arg;
            }
        }
    }
}
