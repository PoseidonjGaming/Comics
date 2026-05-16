using System.ComponentModel;

namespace ComicsInfraLib.Models
{
    public partial class ComicInputModel : INotifyPropertyChanged
    {
        private string _packageName = "";
        public string PackageName
        {
            get => _packageName;
            set
            {
                _packageName = value.Trim();
                OnPropertyChanged(nameof(PackageName));
            }
        }

        private string _author = "";
        public string Author
        {
            get => _author;
            set
            {
                _author = value.Trim();
                OnPropertyChanged(nameof(Author));
            }
        }
        private string _baseURL = "";
        public string BaseURL
        {
            get => _baseURL;
            set
            {
                _baseURL = value.Trim();
                OnPropertyChanged(nameof(BaseURL));
            }
        }

        private int _numberPages = 0;
        public int NumberPages
        {
            get => _numberPages;
            set
            {
                _numberPages = value;
                OnPropertyChanged(nameof(NumberPages));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Reset()
        {
            PackageName = string.Empty;
            Author = string.Empty;
            BaseURL = string.Empty;
            NumberPages = 0;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
