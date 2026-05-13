using ComicsInfraLib.Services;
using FuzzierSharp.Extractor;
using Microsoft.UI.Xaml.Controls.Primitives;
using SearchComicsLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ModernDownladComics.Utilities
{
    public class ContentPageUtility
    {
        public static string Search(string name, string author, string path, string from)
        {
            string authorPath = SearchUtility.GetAuthorPath(author.Trim(), path);
            if (Path.GetFileNameWithoutExtension(authorPath) != author) return "Empty";
            IEnumerable<ExtractedResult<string>> results = SearchUtility.GetComics(authorPath, name.Trim());
            ExtractedResult<string>? res = results.FirstOrDefault();

            if (res == null)
                return "Empty";

            string fromPath = res.Value.Replace(authorPath, string.Empty)[1..];

            return $"From {from}: {SearchUtility.CountPage(res.Value)} pages - {fromPath}";
        }
    }
}
