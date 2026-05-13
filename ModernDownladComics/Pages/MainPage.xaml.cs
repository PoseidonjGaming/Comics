using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Models.View;
using ModernDownladComics.windows;
using System;
using System.Collections.ObjectModel;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; set; }
        public MainPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetRequiredService<MainPageViewModel>();
            ViewModel.ChangeSourceRequested += comic =>
            {
                _ = new ChangeSourceWindow(comic);
            };

            DataContext = ViewModel;

        }
    }
}
