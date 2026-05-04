using HtmlAgilityPack;

namespace ComicsServiceLib
{
    public interface IHtmlParserService
    {
        HtmlNode? LoadBody(string html);
        HtmlNode? FindNode(HtmlNode parent);
        HtmlNode? FindNodeWithAttribute(HtmlNode node, string search, string attribute);
    }
}
