using ModernDownloadComics.Models;
using System.ComponentModel;

namespace ModernDownloadComics.model
{
    public partial class Comic : INotifyPropertyChanged
    {
        public string URL { get; set; } = "";
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

        public string extension = "";
        public string Host { get; set; } = "";
        public int NumberPages { get; set; } = 0;
        public string Author { get; set; } = "";
        public string Path { get; set; } = "";
        public bool Enabled { get; set; } = false;
        public Priority Priority { get; set; } = Priority.DEFAULT;
        public bool DeepAnalyze { get; set; } = true;


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetFilename()
        {
            return $"{Filename}{extension}";
        }

        public void Populate(string url, string baseUrl, string name, string newFilename, string host, int nbPages, string author)
        {
            URL = url;
            BaseURL = baseUrl;
            PackageName = name.Trim();
            Filename = System.IO.Path.GetFileNameWithoutExtension(newFilename);
            extension = System.IO.Path.GetExtension(newFilename);
            Host = host;
            NumberPages = nbPages;
            Author = author;
        }

        public void Populate(string url, string name, string host, string author)
        {
            URL = url;
            BaseURL = url;
            PackageName = name.Trim();
            Host = host;
            Author = author;
        }

        public Comic(string url, string baseURL, string packageName, string filename, string host, int pages, string author)
        {
            this.URL = url;
            this.BaseURL = baseURL;
            this._packageName = packageName.Trim();
            this.Filename = System.IO.Path.GetFileNameWithoutExtension(filename);
            this.extension = System.IO.Path.GetExtension(filename);
            this.Host = host;
            this.NumberPages = pages;
            this.Author = author;
            this.Path = System.IO.Path.Combine(Jdownloader.ComicsDirectory, author.Trim(), packageName.Trim());
        }

        public Comic(string url, string baseUrl, string name, string newFilename, string host, int nbPages, string author, bool enable) :
            this(url, baseUrl, name, newFilename, host, nbPages, author)
        {
            this.Enabled = enable;
        }

        public Comic()
        {

        }
    }
}
