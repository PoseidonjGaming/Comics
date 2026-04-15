namespace DownloadComics.models
{
    public class JDCredentials(string email, string password, string device)
    {
        public string Email { get; set; } = email;
        public string Password { get; set; } = password;
        public string Device { get; set; } = device;
    }
}
