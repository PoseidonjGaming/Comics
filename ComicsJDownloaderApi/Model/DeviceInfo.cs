namespace ComicsJDownloaderApi.Model
{
    public class DeviceInfo
    {
        public string DeviceId { get; set; } = "";

        public string IP { get; set; } = "";

        public int? Port { get; set; }

        public bool AutoFallback { get; set; }

        public bool HasDirectConnectionInfo
        {
            get
            {
                if (!string.IsNullOrEmpty(IP))
                {
                    return Port.HasValue;
                }

                return false;
            }
        }

        public string DirectConnectionUrl => $"http://{IP}:{Port}";
    }
}
