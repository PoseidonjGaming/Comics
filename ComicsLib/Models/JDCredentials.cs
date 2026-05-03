using System.ComponentModel;

namespace ComicsLib.Models
{
    public class JDCredentials(string email, string password, string device) : INotifyPropertyChanged
    {
        private string _email = email;
        public string Email
        {
            get => _email; set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        private string _password = password;
        public string Password
        {
            get => _password; set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        private string _device = device;
        public string Device
        {
            get => _device; set
            {
                if (_device != value)
                {
                    _device = value;
                    OnPropertyChanged(nameof(Device));
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
