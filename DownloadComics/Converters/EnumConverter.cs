using ComicsLocalizationLib;
using DownloadComics.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows.Data;

namespace DownloadComics.Converters
{
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string v = value.ToString() ?? string.Empty;
            return App.Current.ServiceProvider.GetRequiredService<DownloadLocalizationService>()[$"Priority.{v}"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
    }
}
