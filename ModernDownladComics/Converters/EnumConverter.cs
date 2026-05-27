using ComicsLocalizationLib;
using Microsoft.UI.Xaml.Data;
using ModernDownladComics;
using System;
using System.Collections.Generic;

namespace ModernDownloadComics.Converters
{
    public partial class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            Dictionary<string, string> dic = App.Current.LocalizationService.Data["Priority"] as Dictionary<string, string> ?? [];
            string v = value.ToString() ?? string.Empty;

            return dic[v];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        => new();
    }
}
