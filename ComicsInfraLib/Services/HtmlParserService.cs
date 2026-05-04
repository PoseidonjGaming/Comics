using ComicsInfraLib.Helpers;
using ComicsServiceLib;
using HtmlAgilityPack;

namespace ComicsInfraLib.Services
{
    public class HtmlParserService : IHtmlParserService
    {
        public HtmlNode? FindNode(HtmlNode parent)
        {
            foreach (HtmlNode node in parent.ChildNodes)
            {
                string text = node.InnerText;
                if (RegexUtility.PageRegex().IsMatch(text))
                    return node;
                var found = FindNode(node);
                if (found != null) return found;
            }
            return null;
        }

        public HtmlNode? FindNodeWithAttribute(HtmlNode parentNode, string search, string attribute)
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

        public HtmlNode? LoadBody(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            HtmlDocument doc = new();
            doc.LoadHtml(html);
            return doc.DocumentNode.SelectSingleNode("//body");
        }
    }
}
