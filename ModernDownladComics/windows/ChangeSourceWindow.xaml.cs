using System.Collections.ObjectModel;
using ComicsInfraLib.Helpers;
using ComicsInfraLib.Models.Views;
using ComicsLib.Models;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ModernDownloadComics.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChangeSourceWindow : Window
    {
        private static WindowService WindowService => WindowService.Instance;
        public ChangeSourcePageViewModel ViewModel { get; set; }
        public ChangeSourceWindow(Comic comic)
        {
            InitializeComponent();

            ViewModel = App.Current.Services.GetRequiredService<ChangeSourcePageViewModel>();
            ViewModel.Init(comic);
            AppWindow.Resize(new(200, 115));
            AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

            WindowService.SetOwner(this);
            WindowService.Center(this);

            OverlappedPresenter presenter = OverlappedPresenter.Create();
            presenter.IsModal = true;
            presenter.SetBorderAndTitleBar(true, false);

            AppWindow.SetPresenter(presenter);

            AppWindow.Show();
        }

        private async void SelectBTN_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ComicsHostCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.ChangeSource();
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            App.Current.Services.GetRequiredService<IStateRepository>().Save();
        }
    }
}
