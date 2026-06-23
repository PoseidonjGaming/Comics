using ComicsInfraLib.Helpers;
using System.Globalization;
using System.Windows.Data;

namespace DownloadComics.Converters
{
    public class HostConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string host && RegexUtility.HostRegex().IsMatch(host))
            {
                try
                {
                    var uri = new Uri(host);
                    return uri.Host;
                }
                catch
                {
                    return RegexUtility.HostRegex().Match(host).Value
                        .Replace("https://", string.Empty)
                        .Replace("http://", string.Empty).TrimEnd('/');
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
