using ComicsLib.Models;
using ComicsLib.Utilities;
using FuzzierSharp.Extractor;
using HtmlAgilityPack;
using Newtonsoft.Json;
using SearchComicsLib;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ComicsLib.Services
{
    public class ComicService(Action<Comic?> action,
        Action<string, RetrieveSource, TaskCompletionSource<string>> webAction)
    {
        private static AppState State => AppStateStore.Instance;

        private readonly Action<string, RetrieveSource, TaskCompletionSource<string>> _webAction = webAction;
        private readonly Action<Comic?> _pathAction = action;

        private string HtmlBody = "";

        public static string GetHost(string[] hostsSettings)
        {
            Dictionary<string, int> hostCount = [];

            foreach (string host in hostsSettings)
            {
                hostCount.Add(host, State.Comics.Count(c => c.Host == host));
            }

            return hostCount.MinBy(v => v.Value).Key;
        }

        public static string GetUrlByHost(HtmlNode parentNode, string host)
        {
            HtmlNode? node = FindNodeWithAttribute(parentNode, host, "href");
            string? link = node?.GetAttributeValue("href", "no value");

            return link ?? "";
        }

        public static HtmlNode? FindNodeWithAttribute(HtmlNode parentNode, string search, string attribute)
        {
            var children = parentNode.ChildNodes;
            for (int i = 0; i < children.Count; i++)
            {
                var node = children[i];

                if (node.Attributes.Contains(attribute) &&
                    node.GetAttributeValue(attribute, string.Empty).Contains(search, StringComparison.Ordinal))
                {
                    return node;
                }
                var found = FindNodeWithAttribute(node, search, attribute);
                if (found != null) return found;
            }
            return null;

        }

        public async void MakeComic(string baseUrl, string author, string name, int numberPage,
            bool isScan, ISettingsService? optionsService, string? htmlBody = "")
        {
            if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(name))
            {
                return;
            }
            Options? options = optionsService?.GetOptions();

            if (options != null)
            {
                Comic? comic = options.Comic;
                if (comic == null) return;

                if (isScan)
                {
                    HtmlNode? node = await GetBodyNode(baseUrl, htmlBody);

                    if (node != null)
                    {
                        string url;
                        string host;
                        List<string> selectedHost = [.. options.Hosts];
                        do
                        {
                            host = GetHost([.. selectedHost]);

                            string formatHost = host.Replace("https://", string.Empty);
                            formatHost = formatHost.TrimEnd('/');

                            url = GetUrlByHost(node, formatHost);
                            if (string.IsNullOrEmpty(url))
                            {
                                selectedHost.Remove(host);
                            }
                        } while (string.IsNullOrEmpty(url) && selectedHost.Count > 0);

                        TaskCompletionSource<string> source = new();
                        _webAction.Invoke(url, RetrieveSource.URL, source);
                        url = await source.Task;

                        string newFilename;

                        var match = RegexUtility.FilenameRegex().Match(url);

                        if (match.Success)
                        {
                            var value = match.Value;
                            newFilename = value.Length > 2
                                ? value[1..^1].Replace("_", " ")
                                : value;
                        }
                        else
                        {
                            newFilename = name;
                        }


                        comic.Populate(url, baseUrl, name, newFilename, numberPage, author.Trim(), HtmlBody);
                    }
                }
                else
                {
                    comic.Populate(baseUrl, name, RegexUtility.HostRegex().Match(baseUrl).Value, author.Trim(), "");
                    comic.NumberPages = numberPage;
                }



                _pathAction.Invoke(comic);
                
            }


        }

        public static HtmlNode? FindNode(HtmlNode parentNode)
        {
            foreach (HtmlNode node in parentNode.ChildNodes)
            {
                string text = node.InnerText;
                if (RegexUtility.PageRegex().IsMatch(text))
                    return node;
                var found = FindNode(node);
                if (found != null) return found;
            }
            return null;

        }

        public async Task<HtmlNode?> GetBodyNode(string baseUrl, string? baseBody)
        {
            if (string.IsNullOrEmpty(baseBody))
            {
                TaskCompletionSource<string> source = new();
                _webAction.Invoke(baseUrl, RetrieveSource.HTML, source);
                HtmlBody = await source.Task;
            }

            return LoadBody(HtmlBody);
        }

        public static HtmlNode? LoadBody(string? html, string node = "//body")
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            HtmlDocument doc = new();
            doc.LoadHtml(html);
            return doc.DocumentNode.SelectSingleNode(node);
        }

        

        private static void AddArg(Collection<string> args, string key, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                args.Add($"--{key}");
                args.Add(value);
            }
        }

        
    }
}
