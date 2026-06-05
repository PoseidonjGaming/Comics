using ComicsLocalizationLib;
using FuzzierSharp;
using FuzzierSharp.Extractor;
using Microsoft.Extensions.DependencyInjection;
using SearchComicsLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModernDownladComics.Utilities
{
    public class ContentPageUtility
    {
        public static string Search(string name, string author, string path, string from)
        {
            string authorPath = SearchUtility.GetAuthorPath(author, path);
            if (Fuzz.Ratio(Path.GetFileNameWithoutExtension(authorPath), author) < 80)
                return App.Current.Services
                    .GetRequiredService<LocalizationService>()["SearchPage.Empty"];
            IEnumerable<ExtractedResult<string>> results = SearchUtility.GetComics(authorPath, name);
            ExtractedResult<string>? res = results.FirstOrDefault();

            if (res == null)
                return App.Current.Services
                    .GetRequiredService<LocalizationService>()["SearchPage.Empty"];

            string fromPath = res.Value.Replace(authorPath, string.Empty)[1..];
            string fromLoc = App.Current.Services
                .GetRequiredService<LocalizationService>()["SearchPage.From"];
            return $"{fromLoc} {from}: {SearchUtility.CountPage(res.Value)} pages - {fromPath}";
        }
    }
}
