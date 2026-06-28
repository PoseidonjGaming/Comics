using ComicsLocalizationLib;
using DownloadComics.Converters;
using DownloadComics.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace DownloadComics.Extension
{
    public class LocExtension : MarkupExtension
    {
        public string Key { get; set; }
        public string Format { get; set; }
        public string Section { get; set; }


        public LocExtension()
        {
            Key = string.Empty;
            Section = string.Empty;
            Format = string.Empty;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            
           if(DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return $"[{Key}]";
            }
            var binding = new Binding
            {
                Path = new PropertyPath($"[{Section}.{Key}]"),
                Source = App.Current.ServiceProvider.GetRequiredService<ILocalizationService>(),
                Converter = new LocConverter(Format),
                Mode = BindingMode.OneWay
            };
            return binding.ProvideValue(serviceProvider);
        }
    }
}
