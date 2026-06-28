using ComicsLocalizationLib;
using Microsoft.Extensions.DependencyInjection;
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
            string v = value.ToString() ?? string.Empty;
            return App.Current.Services.GetRequiredService<ILocalizationService>().Get($"Priority.{v}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        => new();
    }
}
