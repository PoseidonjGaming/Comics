namespace ComicsJDownloaderApi.Model
{
    public class GetRequestArg(string action, byte[] key)
    {
        public string Action { get; set; } = action;

        public byte[] Key { get; set; } = key;
    }
}
