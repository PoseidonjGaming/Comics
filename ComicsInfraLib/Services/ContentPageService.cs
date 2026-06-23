using ComicsLocalizationLib;
using FuzzierSharp;
using FuzzierSharp.Extractor;
using SearchComicsLib;

namespace ComicsInfraLib.Services
{
    public class ContentPageService<L>(L localizationService) where L : LocalizationService
    {
        public string Search(string name, string author, string path, string from)
        {
            string authorPath = SearchUtility.GetAuthorPath(author, path);
            if (Fuzz.Ratio(Path.GetFileNameWithoutExtension(authorPath.Replace(".", "_")),
                author.Replace(".", "_")) < 80)
                return localizationService["SearchDialog.Empty"];
            IEnumerable<ExtractedResult<string>> results = SearchUtility.GetComics(authorPath, name);
            ExtractedResult<string>? res = results.FirstOrDefault();

            if (res == null)
                return localizationService["SearchDialog.Empty"];

            string fromPath = res.Value.Replace(authorPath, string.Empty)[1..];
            string fromLoc = localizationService["SearchDialog.From"];
            return $"{fromLoc} {from}: {SearchUtility.CountPage(res.Value)} pages - {fromPath}";
        }
    }
}
