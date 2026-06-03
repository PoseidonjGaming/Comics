using ComicsLocalizationLib;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;

namespace ModernDownladComics.Converters
{
    public partial class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string? code = value as string;
            if (!string.IsNullOrEmpty(code))
            {
                BitmapImage image = new();
                image.SetSource(LocalizationService.GetResource(code).AsRandomAccessStream());
                return image;
            }
            return new();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        => new();
    }
}
