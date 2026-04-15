namespace ModernDownloadComics.Models
{
    public class Track(string baseUrl, string downloadURL)
    {
        public string BaseURL { get; set; } = baseUrl;
        public string DownloadURL { get; set; } = downloadURL;
    }
}
