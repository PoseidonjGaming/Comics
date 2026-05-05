using ComicsInfraLib.Helpers;
using ComicsLib.Models;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace ComicsInfraLib.Services
{
    public class ComicsBuilderService(IWebService web, IHtmlParserService parser,
        IHostService hostSelector, ISettingsService settings) : IComicsBuilderService
    {
        public async Task<Comic?> MakeComics(string baseUrl, string author, string name, int numberPage, bool isScan, string? htmlBody = "")
        {
            Task<Comic?> nullResult = Task.FromResult<Comic?>(null);
            if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(name))
            {
                return await nullResult;
            }

            Options options = settings.GetOptions();
            Comic? comic = options.Comic;

            if (comic == null)
            {
                return await Task.FromResult<Comic?>(null);
            }
            comic = comic.Copy();
            if (isScan)
            {
                string html = await web.Resolve(baseUrl, RetrieveSource.HTML);
                HtmlNode? body = parser.LoadBody(html);

                if (body == null)
                    return await nullResult;

                string host = hostSelector.SelectHost(AppStateStore.Instance.Comics,
                    options.Hosts);
                host = host.Replace("https://", string.Empty)
                    .Replace("http://", string.Empty).TrimEnd('/');

                HtmlNode? node = parser.FindNodeWithAttribute(body, host, "href");
                if (node == null)
                    return await nullResult;

                string url = node.GetAttributeValue("href", "");

                if (string.IsNullOrEmpty(url))
                    return await nullResult;

                url = await web.Resolve(url, RetrieveSource.URL);

                Match match = RegexUtility.FilenameRegex().Match(url);
                string filename = match.Success ? match.Value[1..^1].Replace("_s", "'s")
                    .Replace("_", " ") : name;

                comic.Populate(url, baseUrl, name, filename, numberPage, author.Trim(), html);
                comic.Host = RegexUtility.HostRegex().Match(url).Value;
                comic.NumberPages = numberPage;
            }
            else
            {
                comic.Populate(baseUrl, name, RegexUtility.HostRegex().Match(baseUrl).Value, author.Trim(), "");
                comic.Host = RegexUtility.HostRegex().Match(baseUrl).Value;
                comic.NumberPages = numberPage;
            }

            return comic;
        }
    }
}
