using ComicsLocalizationLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using ModernDownladComics.Converters;

namespace ModernDownladComics
{
   partial class LocExtension: MarkupExtension
    {
        public string Key { get; set; }
        public string Format { get; set; }
        public string Section { get; set; }

        public LocExtension()
        {
            Key = string.Empty;
            Format = string.Empty;
            Section = string.Empty;
        }

        public LocExtension(string key, string format)
        {
            Key = key;
            Format = format;
            Section = string.Empty;
        }

        public LocExtension(string key)
        {
            Key = key;
            Format = string.Empty;
            Section = string.Empty;
        }
        protected override object ProvideValue()
        {
            return new Binding()
            {
                Source = App.Current.Services.GetRequiredService<LocalizationService>(),
                Path = new PropertyPath($"[{Section}.{Key}]"),
                Mode = BindingMode.OneWay,
                Converter = new LocConverter(Format)
            };
        }
    }
}
