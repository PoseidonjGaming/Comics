using DownloadComics.models;
using DownloadComics.windows;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace DownloadComics.utilities
{
    public static class ComicUtility
    {
        private static AppState State => AppStateStore.Instance;

        public static string GetHost(string[] hostsSettings)
        {
            Dictionary<string, int> hostCount = [];

            foreach (string host in hostsSettings)
            {
                hostCount.Add(host, State.GetComics().Count(c => c.Host == host));
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

        public static void MakeComic(string baseUrl, string author, string name, int numberPage,
            Window owner, bool isScan, string? htmlBody = "")
        {
            if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(name))
            {
                return;
            }
            Options? options = JsonConvert.DeserializeObject<Options>(Properties.Settings.Default.Options);

            if (options != null)
            {
                Comic? comic = options.Comic;
                if (comic == null) return;

                if (isScan)
                {
                    HtmlNode? node = GetBodyNode(baseUrl, htmlBody, out string? body);

                    if (node != null)
                    {
                        string url = "";
                        string host = "";
                        List<string> selectedHost = [.. options.Hosts];
                        do
                        {
                            host = GetHost([.. selectedHost]);

                            string formatHost = host.Replace("https://", string.Empty);
                            formatHost.TrimEnd("/");

                            url = GetUrlByHost(node, formatHost);
                            if (string.IsNullOrEmpty(url))
                            {
                                selectedHost.Remove(host);
                            }
                        } while (string.IsNullOrEmpty(url) && selectedHost.Count > 0);

                        ResolveCaptcha captcha = new(url, ResolveCaptcha.RetrieveSource.URL, realUrl =>
                        {
                            url = realUrl ?? url;
                        })
                        {
                            Owner = owner
                        };

                        captcha.ShowDialog();

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


                        comic.Populate(url, baseUrl, name, newFilename, numberPage, author.Trim(), body);
                    }
                }
                else
                {
                    comic.Populate(baseUrl, name, RegexUtility.HostRegex().Match(baseUrl).Value, author.Trim(), "");
                    comic.NumberPages = numberPage;
                }

                PathControl pathControl = new(comic)
                {
                    Owner = owner
                };


                bool? result = pathControl.ShowDialog();
                if (result == true)
                {
                    State.AddComic(comic);
                    State.AddTrack(new(comic.BaseURL, comic.URL, comic.Host));
                }
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

        public static HtmlNode? GetBodyNode(string baseUrl, string? baseBody, out string? body)
        {
            string? htmlBody = baseBody;
            if (string.IsNullOrEmpty(baseBody))
            {
                ResolveCaptcha resolveCaptcha = new(baseUrl, ResolveCaptcha.RetrieveSource.HTML, body =>
                {
                    htmlBody = body;
                })
                {
                    Owner = Application.Current.MainWindow
                };

                resolveCaptcha.ShowDialog();
            }



            body = htmlBody;

            return LoadBody(body);
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

        public static void SearchComic(string author, string comic, string? compare = null,
            string? delete = null, string? jd = null)
        {
            Process searchProcess = new()
            {
                StartInfo = new()
                {
                    FileName = Path.Combine(Directory.GetCurrentDirectory(), "Searchcomics.exe")
                },
                
            };

            searchProcess.StartInfo.ArgumentList.Add("--author");
            searchProcess.StartInfo.ArgumentList.Add(author);
            searchProcess.StartInfo.ArgumentList.Add("--comic");
            searchProcess.StartInfo.ArgumentList.Add(comic);

            AddArg(searchProcess.StartInfo.ArgumentList, "compare", compare);
            AddArg(searchProcess.StartInfo.ArgumentList, "delete", delete);
            AddArg(searchProcess.StartInfo.ArgumentList, "jd", jd);

            searchProcess.Start();

        }

        private static void AddArg(Collection<string> args,string key, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                args.Add($"--{key}");
                args.Add(value);
            }
        }
    }
}
