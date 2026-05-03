using Microsoft.UI.Xaml.Data;
using ModernDownloadComics.Resources;
using System;

namespace ModernDownloadComics.Converters
{
    partial class StaticConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return TranslationSource.Instance[$"{value}"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new();
        }
    }
}
