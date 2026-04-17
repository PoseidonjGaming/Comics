using DownloadComics.resources.converters;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace DownloadComics.resources
{
    public class LocExtension: MarkupExtension
    {
        public string Key { get; set; }
        public string Format { get; set; }

        public LocExtension()
        {
            Key = string.Empty;
            Format = string.Empty;
        }

        public LocExtension(string key, string format)
        {
            Key = key;
            Format = format;
        }

        public LocExtension(string key)
        {
            Key = key;
            Format = string.Empty;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Key))
                return $"[{Key}]";


            return new Binding()
            {
                Source = TranslationSource.Instance,
                Path = new PropertyPath($"[{Key}]"),
                Mode = BindingMode.OneWay,
                Converter = new LocConverter(Format)
            }.ProvideValue(serviceProvider);
        }
    }
}
