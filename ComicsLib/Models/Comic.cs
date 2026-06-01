using Newtonsoft.Json;
using System.ComponentModel;

namespace ComicsLib.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class Comic : INotifyPropertyChanged
    {
        public long UUID { get; set; }

        private string _url;

        [JsonProperty]
        public string URL
        {
            get { return _url; }
            set
            {
                if (_url != value) {
                    _url = value;
                    OnPropertyChanged(nameof(URL));
                }
                
            }
        }
        private string _baseURL;

        [JsonProperty]
        public string BaseURL
        {
            get => _baseURL;
            set
            {
                if (_baseURL != value) {
                    _baseURL = value;
                    OnPropertyChanged(nameof(BaseURL));
                }
                
            }
        }


        private string _packageName = "";
        [JsonProperty]
        public string PackageName
        {
            get => _packageName;
            set
            {
                if (_packageName != value)
                {
                    _packageName = value.Trim();
                    OnPropertyChanged(nameof(PackageName));
                }
            }
        }

        private string _filename;
        [JsonProperty]
        public string Filename { 
            get => _filename;
            set
            {
                if (_filename != value)
                {
                    _filename = value.Trim();
                    OnPropertyChanged(nameof(Filename));
                }
            }
        }

        public string Extansion { get; set; } = "";

        private string _host = "";
        [JsonProperty]
        public string Host
        {
            get => _host;
            set
            {
                if (_host != value)
                {
                    _host = value;
                    OnPropertyChanged(nameof(Host));
                }
            }
        }

        private int _numberPages = 0;
        [JsonProperty]
        public int NumberPages
        {
            get => _numberPages;
            set
            {
                _numberPages = value;
                OnPropertyChanged(nameof(NumberPages));
            }
        }

        private string _author = "";
        [JsonProperty]
        public string Author
        {
            get => _author;
            set
            {
                if (_author!= value)
                {
                    _author = value.Trim();
                    OnPropertyChanged(nameof(Author));
                }
               
            }
        }

        [JsonProperty]
        public string Path { get; set; } = "";

        [JsonProperty]
        public bool Enabled { get; set; } = false;

        private Priorities _priority;
        [JsonProperty]
        public Priorities Priority
        {
            get => _priority;
            set
            {
                _priority = value;
                OnPropertyChanged(nameof(Priority));
            }
        }
        public bool DeepAnalyze { get; set; } = true;

        private string? _htmlBody;
        [JsonProperty]
        public string? HtmlBody
        {
            get => _htmlBody;
            set
            {
                if (_htmlBody != value)
                {
                    _htmlBody = value;
                    OnPropertyChanged(nameof(HtmlBody));
                }

            }
        }


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

        public Comic(string url, string baseURL, string packageName, string filename, string host, int pages, string author)
        {
            this._url = url;
            this._baseURL = baseURL;
            this._packageName = packageName.Trim();
            this.Filename = System.IO.Path.GetFileNameWithoutExtension(filename);
            this.Extansion = System.IO.Path.GetExtension(filename);
            this.Host = host;
            this.NumberPages = pages;
            this.Author = author;
            this.Path = System.IO.Path.Combine(author.Trim(), packageName.Trim());
            this._filename = filename;
        }

        public Comic()
        {
            _url = "";
            _baseURL = "";
            _filename = "";
        }

        public void Reset()
        {
            BaseURL = "";
            PackageName = "";
            NumberPages = 0;
            Author = "";
        }

        public Comic Copy()
        {
            return new Comic
            {
                UUID = this.UUID,
                URL = this.URL,
                BaseURL = this.BaseURL,
                PackageName = this.PackageName,
                Filename = this.Filename,
                Extansion = this.Extansion,
                Host = this.Host,
                NumberPages = this.NumberPages,
                Author = this.Author,
                Path = this.Path,
                Enabled = this.Enabled,
                Priority = this.Priority,
                DeepAnalyze = this.DeepAnalyze,
                HtmlBody = this.HtmlBody
            };
        }
    }
}
