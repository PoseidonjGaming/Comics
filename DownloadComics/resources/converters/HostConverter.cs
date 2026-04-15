using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Policy;
using System.Text;
using System.Windows.Data;

namespace DownloadComics.resources.converters
{
    class HostConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string host)
            {
                try
                {
                    var uri = new Uri(host);
                    return uri.Host;
                }
                catch
                {
                    return host.Replace("https://", string.Empty)
                        .Replace("http://", string.Empty).TrimEnd('/');
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
