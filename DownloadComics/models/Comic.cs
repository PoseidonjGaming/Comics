using DownloadComics.services;
using DownloadComics.utilities;
using System.ComponentModel;

namespace DownloadComics.models
{
    public partial class Comic : INotifyPropertyChanged
    {
        public static Comic FallBack = new Comic(
                "https://exemple_url.com",
                "https://exemple_baseurl.com",
                "package name",
                "filename",
                "host.com",
                0,
                "author");
        public long UUID { get; set; }
        public List<long> LinksUUId { get; set; } = [];

        private string _url;
        public string URL
        {
            get { return _url; }
            set
            {
                _url = value;
                Host = RegexUtility.HostRegex().Match(value).Value;
            }
        }
        public string BaseURL { get; set; } = "";

        private string _packageName = "";
        public string PackageName
        {
            get => _packageName;
            set
            {
                if (_packageName != value)
                {
                    _packageName = value;
                    OnPropertyChanged(nameof(PackageName));
                }
            }
        }

        public string Filename { get; set; } = "";

        public string Extansion { get; set; } = "";

        public string Host { get; set; } = "";
        public int NumberPages { get; set; } = 0;
        public string Author { get; set; } = "";
        public string Path { get; set; } = "";
        public bool Enabled { get; set; } = false;
        public Priority Priority { get; set; } = Priority.DEFAULT;
        public bool DeepAnalyze { get; set; } = true;

        public string? HtmlBody { get; set; } = "";

        public bool AutoConfirm { get; set; } = false;


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetFilename()
        {
            return $"{Filename.Trim()}{Extansion}";
        }

        public void Populate(string url, string baseUrl, string name, string newFilename, int nbPages, string author, string? body)
        {
            URL = url;
            BaseURL = baseUrl;
            PackageName = name.Trim();
            Filename = System.IO.Path.GetFileNameWithoutExtension(newFilename);
            NumberPages = nbPages;
            Author = author;
            HtmlBody = body;
        }

        public void Populate(string url, string name, string host, string author, string body)
        {
            URL = url;
            BaseURL = url;
            PackageName = name.Trim();
            Host = host;
            Author = author;
            HtmlBody = body;
        }

        public bool ShoudSerializeHtmlBody()
        {
            return false;
        }

        public Comic(string url, string baseURL, string packageName, string filename, string host, int pages, string author)
        {
            this._url = url;
            this.BaseURL = baseURL;
            this._packageName = packageName.Trim();
            this.Filename = System.IO.Path.GetFileNameWithoutExtension(filename);
            this.Extansion = System.IO.Path.GetExtension(filename);
            this.Host = host;
            this.NumberPages = pages;
            this.Author = author;
            this.Path = System.IO.Path.Combine(FileService.ComicsDirectory, author.Trim(), packageName.Trim());
        }

        public Comic()
        {
            _url = "";
        }
    }
}
