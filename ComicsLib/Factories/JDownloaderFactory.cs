using ComicsLib.Models;
using ComicsLib.Services;
using JDownloader;
using JDownloader.Model;

namespace ComicsLib.Factories
{
    public static class JDownloaderFactory
    {
        public static async Task<JDownloaderClient> CreateAsnc(ICredentialsService credentialsService)
        {
            JDownloaderClient client = new(new()
            {
                AppKey = "ModernDownloadComics"
            });

            try
            {
                JDCredentials credentials = credentialsService.GetCredentials();

                if (!client.IsConnected)
                {
                    await client.Connect(credentials.Email, credentials.Password);
                    DeviceList devices = await client.ListDevices();
                    DeviceData? targetDevice = devices.Devices
                        .FirstOrDefault(d => d.Name == credentials.Device);

                    if (targetDevice != null)
                    {
                        client.SetWorkingDevice(targetDevice);

                        DirectConnectionInfos? directInfos = await client.Device.GetDirectConnectionInfos();
                        if (directInfos.Infos.Count > 0)
                        {
                            client.SetDirectConnectionInfo(directInfos.Infos[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return client;
        }
    }
}

