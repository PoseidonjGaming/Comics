using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace ModernDownloadComics.Converters
{
    public partial class LocConverter(string format) : IValueConverter
    {
        private readonly string _format = format;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (string.IsNullOrEmpty(_format))
                return value;

            return string.Format(_format, value);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
       => new();

    }
}
