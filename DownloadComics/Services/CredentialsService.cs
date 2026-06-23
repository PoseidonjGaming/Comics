using ComicsLib.Models;
using ComicsServiceLib;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace DownloadComics.Services
{
    public class CredentialsService : ICredentialsService
    {
        private readonly Lazy<JDCredentials> _credentials;
        public JDCredentials Credentials => _credentials.Value;

        public CredentialsService()
        {
            _credentials = new(LoadCredentials);
        }

        private static JDCredentials LoadCredentials()
        {
            JDCredentials defaultCredentials = new("", "", "");

            if (string.IsNullOrEmpty(Properties.Settings.Default.JDCredentials))
                return defaultCredentials;

            string baseCredentials = Properties.Settings.Default.JDCredentials;

            byte[] bytes = Convert.FromBase64String(baseCredentials);
            byte[] unprotected = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);

            string json = Encoding.UTF8.GetString(unprotected);
            return JsonConvert.DeserializeObject<JDCredentials>(json) ?? defaultCredentials;
        }

        public JDCredentials GetCredentials()
        {
            return _credentials.Value;
        }

        public void SetCredentials(string email, string password, string device)
        {
            Credentials.Email = email;
            Credentials.Password = password;
            Credentials.Device = device;
        }

        public void SaveCredentials()
        {
            string json = JsonConvert.SerializeObject(_credentials.Value);

            byte[] credentialsData = ProtectedData.Protect(Encoding.UTF8.GetBytes(json), null,
                DataProtectionScope.CurrentUser);
            string baseCredentials = Convert.ToBase64String(credentialsData);

            Properties.Settings.Default.JDCredentials = baseCredentials;
            Properties.Settings.Default.Save();
        }
    }
}
