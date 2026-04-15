using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Downloader
{
    public class JDKeys
    {
        public byte[] AesKey { get; }
        public byte[] HmacKey { get; }

        public JDKeys(string email, string password)
        {
            var emailBytes = Encoding.UTF8.GetBytes(email.ToLower());
            var passBytes = Encoding.UTF8.GetBytes(password);

            var loginSecret = Sha256(emailBytes.Concat(passBytes).ToArray());
            var deviceSecret = Sha256(loginSecret.Concat(emailBytes).ToArray());

            AesKey = deviceSecret.Take(16).ToArray();
            HmacKey = deviceSecret.Skip(16).Take(16).ToArray();
        }

        public static byte[] Sha256(byte[] data)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(data);
        }


    }
}
