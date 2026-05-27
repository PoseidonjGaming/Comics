using Microsoft.UI.Xaml.Data;
using System;

namespace ModernDownloadComics.Converters
{
    public partial class LabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string format = parameter as string ?? "{0}";
            return string.Format(format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        => new();
    }
}
