using ComicsJDownloaderApi;
using ComicsLib.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JDownloader.Model;
using System.Drawing;

namespace ComicsInfraLib.Models.Views
{
    public partial class SettingsCredientialsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial JDCredentials Credentials { get; set; }

        [ObservableProperty]
        public partial string ConnectionLabel { get; set; }

        public event Action? ConnectionEvent;
        public event Action<string>? DialogEvent;

        public SettingsCredientialsPageViewModel()
        {
            Credentials = new("mail", "password", "device");
            ConnectionLabel = string.Empty;
        }

        public void Setup(SettingsPageArgs<JDCredentials> args)
        {
            Credentials = args.Arg;
        }

        [RelayCommand]
        public async Task TestConnection()
        {
            if (ConnectionEvent == null) return;
            try
            {
                ComicsJDownloaderClient client = new(true);

                if (!client.IsConnected)
                {
                    await client.Connect(Credentials.Email, Credentials.Password);

                    DeviceList devices = await client.ListDevices();
                    DeviceData? targetDevice = devices.Devices
                        .FirstOrDefault(d => d.Name == Credentials.Device);

                    if (targetDevice != null)
                    {
                        client.SetWorkingDevice(targetDevice);

                        var directInfos = await client.Device.GetDirectConnectionInfos();
                        if (directInfos.Infos.Count > 0)
                        {
                            client.SetDirectConnectionInfo(directInfos.Infos[0]);
                        }

                        ConnectionLabel = "Connection Seccessful";
                        ConnectionEvent();
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
            if (ConnectionEvent == null || DialogEvent == null) return;
            ConnectionLabel = "Connection Failed";
            ConnectionEvent();
            DialogEvent(message);


        }
    }
}
