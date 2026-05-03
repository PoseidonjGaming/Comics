using ComicsLib.Models;
using ComicsLib.Services;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ModernDownladComics.Services
{
    public class CredentialsService : ICredentialsService
    {
        private readonly Lazy<JDCredentials> _credentials;
        public JDCredentials Credentials => _credentials.Value;

        private readonly string _credentialsPath;
        private readonly string _credentialsDirectory;

        public CredentialsService()
        {
            _credentialsDirectory = Path.Combine(FileService.CurrentDir, "Settings");
            _credentialsPath = Path.Combine(_credentialsDirectory, $"{Environment.UserName}_credentials.json");
            _credentials = new(LoadCredentials);
        }

        private JDCredentials LoadCredentials()
        {
            JDCredentials defaultCredentials = new("", "", "");
            if (!File.Exists(_credentialsPath))
            {
                return defaultCredentials;
            }

            FileService.CreateFolder(_credentialsDirectory);
            string baseCredential = File.ReadAllText(_credentialsPath);
            
            byte[] bytes = Convert.FromBase64String(baseCredential);
            byte[] unprotected = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
            string json = Encoding.UTF8.GetString(unprotected);

            return JsonConvert.DeserializeObject<JDCredentials>(json)?? defaultCredentials;
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

            byte[] credentialsData = ProtectedData.Protect(Encoding.UTF8.GetBytes(json),
                null, DataProtectionScope.CurrentUser);

            string baseCredentials = Convert.ToBase64String(credentialsData);
            
            FileService.CreateFolder(_credentialsDirectory);
            File.WriteAllText(_credentialsPath, baseCredentials);
        }

      
    }
}
