using ComicsLib.Models;
using JDownloader;
using JDownloader.Model;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ModernDownladComics.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsCredentials : Page
    {
        public JDCredentials JDCredentials { get; set; }
        public SettingsCredentials()
        {
            InitializeComponent();
            JDCredentials = new("mail", "password", "device");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is SettingsPageArgs<JDCredentials> args)
            {
                JDCredentials = args.Arg;
            }
        }

        private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                JDownloaderClient client = new(new()
                {
                    AppKey = "DownloadComicsTest"
                });

                if (!client.IsConnected)
                {
                    await client.Connect(JDCredentials.Email, JDCredentials.Password);

                    DeviceList devices = await client.ListDevices();
                    DeviceData? targetDevice = devices.Devices
                        .FirstOrDefault(d => d.Name == JDCredentials.Device);

                    if (targetDevice != null)
                    {
                        client.SetWorkingDevice(targetDevice);

                        var directInfos = await client.Device.GetDirectConnectionInfos();
                        if (directInfos.Infos.Count > 0)
                        {
                            client.SetDirectConnectionInfo(directInfos.Infos[0]);
                        }

                        connectionLBL.Text = "Connection Seccessful";
                        connectionLBL.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        throw new ArgumentException("Email and/or password and/or device name are wrong");
                    }
                }


            }
            catch (MyJDownloaderException ex)
            {
                await HandleConnectionException(ex.Message);
            }
            catch (ArgumentException ex)
            {
                await HandleConnectionException(ex.Message);
            }
        }
        private async Task HandleConnectionException(string message)
        {
            connectionLBL.Text = "Connection Failed";
            connectionLBL.Foreground = new SolidColorBrush(Colors.Red);

            ContentDialog dialog = new()
            {
                Title = "Error",
                Content = message,
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = "Close",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }

    }
}
