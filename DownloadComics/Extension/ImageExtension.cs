using DownloadComics.Converters;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace DownloadComics.Extension
{
    public partial class ImageExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new Binding()
            {
                Path = new PropertyPath("Code"),
                Converter = new ImageConverter(),
            };
        }
    }
}
