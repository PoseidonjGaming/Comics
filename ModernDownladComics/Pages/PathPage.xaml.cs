using ComicsInfraLib.Helpers;
using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib.UI;
using FuzzierSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ModernDownladComics.Models;
using ModernDownladComics.Models.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PathPage : Page
    {

        public PathPageViewModel ViewModel { get; set; }


        public PathPage()
        {
            InitializeComponent();
            ViewModel = App.Current.Services.GetRequiredService<PathPageViewModel>();
            ViewModel.NavigateEvent += (returnType) =>
            {
                Frame.Navigate(returnType);
            };
            ViewModel.AddPathEvent += (paths, item) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    paths.Add(item);
                });
            };

            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is PathPageArgs args)
            {
                ViewModel.Init(args);
            }
        }

        private async void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.Load(() => IsEnabled, state => IsEnabled = state);
        }



        private void Page_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.Unload();
        }
    }
}
