using ComicsJDownloaderApi;
using ComicsLib.Models;
using ComicsLocalizationLib;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JDownloader.Model;

namespace ComicsInfraLib.Models.Views
{
    public partial class SettingsCredientialsPageViewModel<T, L>(IDialogService<T> dialogService,
        L localizationService) : ObservableObject where T : class where L : LocalizationService
    {
        [ObservableProperty]
        public partial JDCredentials Credentials { get; set; } = new("mail", "password", "device");

        [ObservableProperty]
        public partial string ConnectionLabel { get; set; } = string.Empty;


        public event Action<bool>? ConnectionEvent;

        public void Setup(SettingsPageArgs<JDCredentials> args)
        {
            Credentials = args.Arg;
        }

        [RelayCommand]
        public async Task TestConnection(T arg)
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

                        ConnectionLabel = localizationService["SettingsCredentialsPage.ConnectionSuccessful"];
                        ConnectionEvent(true);
                    }
                    else
                    {
                        throw new ArgumentException("Email and/or password and/or device name are wrong");
                    }
                }
            }
            catch (MyJDownloaderException ex)
            {
                await HandleConnectionException(ex.Message, arg);
            }
            catch (ArgumentException ex)
            {
                await HandleConnectionException(ex.Message, arg);
            }
        }

        private async Task HandleConnectionException(string message, T arg)
        {
            if (ConnectionEvent == null) return;
            ConnectionLabel = localizationService["SettingsCredentialsPage.ConnectionFailed"];
            ConnectionEvent(false);
            await dialogService.ShowErrorAsync(arg, message);


        }
    }
}
