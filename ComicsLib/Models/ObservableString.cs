using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicsLib.Models
{
    public class ObservableString: INotifyPropertyChanged
    {
		private string _value;

		public string Value
		{
			get => _value;

            set {
				if (_value != value) {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
			}
		}

        public ObservableString(string startValue)
        {
            _value= startValue;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
