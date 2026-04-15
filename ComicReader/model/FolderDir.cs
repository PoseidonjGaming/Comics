using System.ComponentModel;

namespace ComicReader.model
{
    public partial class FolderDir(string path) : INotifyPropertyChanged
    {
        
        public string Path { get; set; } = path;

        private string _name = System.IO.Path.GetFileName(path);
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
