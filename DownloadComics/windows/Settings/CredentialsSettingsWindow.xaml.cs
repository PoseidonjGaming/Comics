using ComicsInfraLib.Models;
using ComicsInfraLib.Models.Views;
using ComicsLib.Models;
using ComicsServiceLib;
using DownloadComics.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DownloadComics.windows.Settings
{
    /// <summary>
    /// Logique d'interaction pour CredentialsSettingsWindow.xaml
    /// </summary>
    public partial class CredentialsSettingsWindow : Window
    {
        public SettingsCredientialsPageViewModel<Window> ViewModel { get; set; }
        public CredentialsSettingsWindow(SettingsPageArgs<JDCredentials> arg)
        {
            InitializeComponent();
            ViewModel = App.Current.ServiceProvider
                .GetRequiredService<SettingsCredientialsPageViewModel<Window>>();
            ViewModel.Setup(arg);
            ViewModel.ConnectionEvent += (success) =>
            {
                connectionLBL.Foreground = new SolidColorBrush(success ? Colors.Green : Colors.Red);
            };
            DataContext = ViewModel;
            passwordTXT.Password = ViewModel.Credentials.Password;
        }

        private void PasswordTXT_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Credentials.Password != passwordTXT.Password)
                ViewModel.Credentials.Password = passwordTXT.Password;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.ServiceProvider.GetRequiredService<ICredentialsService>().SaveCredentials();
        }
    }
}
