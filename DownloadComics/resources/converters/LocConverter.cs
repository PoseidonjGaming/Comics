using System.Globalization;
using System.Windows.Data;

namespace DownloadComics.resources.converters
{
    public class LocConverter(string format) : IValueConverter
    {
        private readonly string _format = format;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(_format))
                return value;

            return string.Format(_format, value);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
       => Binding.DoNothing;

    }
}
