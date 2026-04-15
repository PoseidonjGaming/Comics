// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

Console.WriteLine("Hello, World!");

await Task.Run(async () =>
{
   string token = await ConnectAsync("lucasmagnier@free.fr", "4pCMq%JDn#KBK3Qf^aSG");
});

static byte[] EncryptAes(byte[] key, byte[] iv, byte[] data)
{
    using var aes = Aes.Create();
    aes.Key = key;
    aes.IV = iv;
    aes.Mode = CipherMode.CBC;
    aes.Padding = PaddingMode.PKCS7;

    using var encryptor = aes.CreateEncryptor();
    return encryptor.TransformFinalBlock(data, 0, data.Length);
}

static byte[] HmacSha256(byte[] key, byte[] data)
{
    using var hmac = new HMACSHA256(key);
    return hmac.ComputeHash(data);
}

static async Task<string> ConnectAsync(string email, string password)
{
    // 1. Derive keys
    var emailBytes = Encoding.UTF8.GetBytes(email.ToLower());
    var passBytes = Encoding.UTF8.GetBytes(password);

    var loginSecret = Sha256(emailBytes.Concat(passBytes).ToArray());
    var deviceSecret = Sha256(loginSecret.Concat(emailBytes).ToArray());

    var aesKey = deviceSecret.Take(16).ToArray();
    var hmacKey = deviceSecret.Skip(16).Take(16).ToArray();

    // 2. Build challenge
    var challenge = Encoding.UTF8.GetBytes("challenge");
    var iv = RandomNumberGenerator.GetBytes(16);
    var encrypted = EncryptAes(aesKey, iv, challenge);
    var signature = HmacSha256(hmacKey, encrypted);

    var passwordField = Convert.ToBase64String(iv.Concat(encrypted).Concat(signature).ToArray());

    // 3. Build URL
    var url = "https://api.jdownloader.org/my/connect?email="
              + WebUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(email)))
              + "&appkey="
              + WebUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes("MonClientCSharp")));

    // 4. Send request
    using var http = new HttpClient();
    var content = new StringContent(
        JsonSerializer.Serialize(new { password = passwordField }),
        Encoding.UTF8,
        "application/json"
    );

    var resp = await http.PostAsync(url, content);
    var respJson = await resp.Content.ReadAsStringAsync();
    Console.WriteLine(respJson);

    var doc = JsonDocument.Parse(respJson);
    return doc.RootElement.GetProperty("sessiontoken").GetString();

}

static byte[] Sha256(byte[] data)
{
    using var sha = SHA256.Create();
    return sha.ComputeHash(data);
}


