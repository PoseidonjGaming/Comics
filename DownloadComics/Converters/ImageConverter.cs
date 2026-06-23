using ComicsLocalizationLib;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DownloadComics.Converters
{
    public partial class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? code = value as string;
            if (!string.IsNullOrEmpty(code))
            {
                BitmapImage image = new();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = LocalizationService.GetResource(code);
                image.EndInit();
                image.Freeze();
                return image;
            }
            return new();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => new();
    }
}
