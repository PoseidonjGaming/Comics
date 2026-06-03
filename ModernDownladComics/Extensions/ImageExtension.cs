using ComicsLocalizationLib;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media.Imaging;
using ModernDownladComics.Converters;
using System.IO;

namespace ModernDownladComics.Extensions
{
    public partial class ImageExtension : MarkupExtension
    {
        protected override object ProvideValue()
        {
            return new Binding()
            {
                Path = new PropertyPath("Code"),
                Converter = new ImageConverter(),
            };
        }
    }
}
