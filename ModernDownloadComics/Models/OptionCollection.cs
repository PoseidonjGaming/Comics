using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ModernDownloadComics.Models
{
    public class OptionCollection : INotifyPropertyChanged
    {
        public ObservableCollection<string> collections = [];

        public event PropertyChangedEventHandler? PropertyChanged;

        private string _optionName = "";
        public string OptionName
        {
            get => _optionName;
            set
            {
                if (_optionName != value)
                {
                    _optionName = value;
                    OnPropertyChanged(nameof(OptionName));
                }
            }
        }

        public OptionCollection(string name)
        {
            OptionName = name;
        }

        public void Feed(string[] strings)
        {
            foreach (string? str in strings)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    collections.Add(str);
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
