using Microsoft.UI.Xaml.Data;
using ModernDownloadComics.Resources;
using System;
using System.Globalization;

namespace ModernDownloadComics.Converters
{
    public partial class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            string v = value.ToString() ?? string.Empty;

            return TranslationSource.Instance[$"Priority_{char.ToUpper(v[0])}{v[1..].ToLower()}"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        => new();
    }
}
