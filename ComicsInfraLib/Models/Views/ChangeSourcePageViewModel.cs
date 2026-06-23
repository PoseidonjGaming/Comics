using System.Collections.ObjectModel;
using ComicsInfraLib.Helpers;
using ComicsLib.Models;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using HtmlAgilityPack;

namespace ComicsInfraLib.Models.Views
{
    public partial class ChangeSourcePageViewModel(IHtmlParserService htmlParserService,
        ISettingsService settingsService) : ObservableObject
    {
        public ObservableCollection<string> Hosts { get; set; } = [];

        [ObservableProperty]
        public partial Comic Comic { get; set; }

        public void ChangeSource()
        {
            HtmlNode? bodyNode = htmlParserService.LoadBody(Comic.HtmlBody ?? "");
            if (bodyNode != null)
            {
                HtmlNode? node = htmlParserService.FindNodeWithAttribute(bodyNode, Comic.Host, "href");
                if (node != null)
                {
                    string newUrl = node.GetAttributeValue("href", "");
                    Comic.URL = newUrl;
                    Comic.Host = RegexUtility.HostRegex().Match(newUrl).Value;
                }
            }
        }

        public void Init(Comic comic)
        {
            foreach (var host in settingsService.GetOptions().Hosts)
            {
                Hosts.Add(host);
            }

            Comic = comic;
        }
    }
}
