using DownloadComics.models;
using DownloadComics.resources.credentialsSettings;
using JDownloader;
using JDownloader.Model;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace DownloadComics.windows.settings
{
    /// <summary>
    /// Logique d'interaction pour CredentialsSettings.xaml
    /// </summary>
    public partial class CredentialsSettings : Window
    {
        public CredentialsSettings()
        {
            InitializeComponent();
        }

        private async void ConnectionTestBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JDownloaderClient client = new(new()
                {
                    AppKey = "DownloadComicsTest"
                });

                if (!client.IsConnected)
                {
                    await client.Connect(emailTXT.Text, passwordTXT.Password);

                    DeviceList devices = await client.ListDevices();
                    DeviceData? targetDevice = devices.Devices
                        .FirstOrDefault(d => d.Name == deviceNameTXT.Text);

                    if (targetDevice != null)
                    {
                        client.SetWorkingDevice(targetDevice);

                        var directInfos = await client.Device.GetDirectConnectionInfos();
                        if (directInfos.Infos.Count > 0)
                        {
                            client.SetDirectConnectionInfo(directInfos.Infos[0]);
                        }

                        connectionLBL.Content = CredentialsSettingsStrings.CredentialsSettings_Connection_Info_Label_On;
                        connectionLBL.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        throw new ArgumentException(CredentialsSettingsStrings.CredentialsSettings_Connection_Error);
                    }
                }


            }
            catch (MyJDownloaderException ex)
            {
                HandleConnectionException(ex.Message);
            }
            catch (ArgumentException ex)
            {
                HandleConnectionException(ex.Message);
            }

        }
        private void HandleConnectionException(string message)
        {
            connectionLBL.Content = CredentialsSettingsStrings.CredentialsSettings_Connection_Info_Label_Off;
            connectionLBL.Foreground = new SolidColorBrush(Colors.Red);

            MessageBox.Show(message);
        }

        private void SaveCredentialsBTN_Click(object sender, RoutedEventArgs e)
        {
            JDCredentials credentials = new(emailTXT.Text, passwordTXT.Password, deviceNameTXT.Text);

            string jsonCredentials = JsonConvert.SerializeObject(credentials);

            byte[] credentialsData = ProtectedData.Protect(Encoding.UTF8.GetBytes(jsonCredentials), null,
                DataProtectionScope.CurrentUser);

            string baseCredentials = Convert.ToBase64String(credentialsData);

            Properties.Settings.Default.JDCredentials = baseCredentials;
            Properties.Settings.Default.Save();

            MessageBox.Show(CredentialsSettingsStrings.Msg_Settings_Saved, CredentialsSettingsStrings.Msg_Settings_Saved_Title,
                        MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
        }
    }
}
