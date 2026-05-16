namespace ComicsJDownloaderApi.Model
{
    public class SessionInfo
    {
        public string? SessionToken { get; set; }

        public string? RegainToken { get; set; }

        public byte[]? ServerEncryptionToken { get; set; }

        public byte[]? DeviceEncryptionToken { get; set; }

        public byte[]? DeviceSecret { get; set; }

        public void Clear()
        {
            SessionToken = null;
            RegainToken = null;
            ServerEncryptionToken = null;
            DeviceEncryptionToken = null;
            DeviceSecret = null;
        }
    }
}
