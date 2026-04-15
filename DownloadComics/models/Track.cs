namespace DownloadComics.models
{
    public class Track
    {
        public string BaseURL { get; set; } = "";
        public string DownloadURL { get; set; } = "";
        public List<string> TestedHost { get; set; } = [];

        public Track(string baseUrl, string downloadURL, string host)
        {
            BaseURL = baseUrl;
            DownloadURL = downloadURL;
            TestedHost.Add(host);
        }

        public Track() { }


    }
}
