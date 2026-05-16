using System.Security.Cryptography;
using System.Text;

namespace ComicsJDownloaderApi.Helpers
{
    public static class JDownloaderCrypto
    {
        public static byte[] GetSecret(string email, string password, string domain)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(email.ToLower() + password + domain);
            return SHA256.HashData(bytes);
        }

        public static byte[] GetEncryptionToken(byte[] secret, string sessionToken)
        {
           
            byte[] second = [..  Enumerable.Range(0, sessionToken.Length).Where(x=>x%2==0)
                .Select(x=>Convert.ToByte(sessionToken.Substring(x, 2), 16))];
            return SHA256.HashData([.. secret, .. second]);
        }

        public static string GetSignature(string data, byte[] key)
        {
            return Convert.ToHexString(new HMACSHA256(key)
                .ComputeHash(Encoding.UTF8.GetBytes(data)))
                .Replace("-", string.Empty).ToLower();
        }

        public static async Task<string> Encrypt(string data, byte[] key)
        {
            using Aes aes = Aes.Create();
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.IV = key.Take(key.Length / 2).ToArray();
            aes.Key = key.Skip(key.Length / 2).ToArray();
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            using MemoryStream memoryStream = new();
            using (CryptoStream cryptoStream = new(memoryStream, encryptor, 
                CryptoStreamMode.Write))
            {
                using StreamWriter streamWriter = new(cryptoStream);
                await streamWriter.WriteAsync(data);
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public static async Task<string> Decrypt(string data, byte[] key)
        {
            byte[] buffer = Convert.FromBase64String(data);
            using Aes aes = Aes.Create();
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.IV = [.. key.Take(key.Length / 2)];
            aes.Key = [.. key.Skip(key.Length / 2)];
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            using MemoryStream memoryStream = new(buffer);
            using CryptoStream cryptoStream = new(memoryStream, decryptor,
                CryptoStreamMode.Read);
            using StreamReader streamReader = new(cryptoStream);
            return await streamReader.ReadToEndAsync();
        }
    }
}
