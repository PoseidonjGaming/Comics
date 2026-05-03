using ComicsLib.Utilities;
using Microsoft.UI.Xaml.Data;
using System;

namespace ModernDownloadComics.Converters
{
    partial class HostConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string host && RegexUtility.HostRegex().IsMatch(host))
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

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new();
        }
    }
}
