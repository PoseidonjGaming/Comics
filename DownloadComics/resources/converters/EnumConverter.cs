using System.Globalization;
using System.Windows.Data;

namespace DownloadComics.resources.converters
{
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string v = value.ToString() ?? string.Empty;

            return TranslationSource.Instance[$"Priority_{char.ToUpper(v[0])}{v[1..].ToLower()}"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
    }
}
