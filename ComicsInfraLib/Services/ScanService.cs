using ComicsInfraLib.Helpers;
using ComicsLib.Models;
using ComicsServiceLib;
using FuzzierSharp;
using FuzzierSharp.Extractor;

namespace ComicsInfraLib.Services
{
    public class ScanService : IScanService
    {
        public int CountPages(string path)
        {
            if (!Directory.Exists(path))
                return 0;

            int pages = 0;
            var dirs = new Stack<string>();
            dirs.Push(path);

            while (dirs.Count > 0)
            {
                var current = dirs.Pop();
                try
                {
                    using (var en = Directory.EnumerateDirectories(current).GetEnumerator())
                    {
                        while (en.MoveNext())
                            pages++;
                    }

                    foreach (var dir in Directory.EnumerateDirectories(current))
                    {
                        dirs.Push(dir);
                    }
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
            return pages;
        }

        public string? FindExactComic(string packageName, IEnumerable<string> directories)
        {
            ExtractedResult<string> match = Process.ExtractOne(packageName, directories, s => Path.GetFileName(s).ToLower());
            return match?.Score == 100 ? match.Value : null;
        }

        public string? FindPreviousChapter(string packageName, string root, IEnumerable<string> directories)
        {
            if (int.TryParse(RegexUtility.ChapterRegex().Match(packageName).Value, out int numChapter))
            {
                numChapter--;

                string prevChapter = RegexUtility.ChapterRegex().Replace(packageName, numChapter.ToString());
                var comicsResult = Process.ExtractOne(prevChapter, directories, Path.GetFileName, cutoff: 100);

                if (comicsResult != null)
                {
                    return comicsResult.Value.Replace(root, string.Empty)
                    .Replace(prevChapter, packageName)[1..];
                }
                else if (numChapter >= 1)
                {
                    FindPreviousChapter(prevChapter, root, directories);
                }
            }
            return null;
        }

        private static string? FindExactAuthor(string author, string root)
        {
            var dirs = Directory.EnumerateDirectories(root);
            var match = Process.ExtractOne(author, dirs, s => Path.GetFileName(s).ToLower());
            return match?.Score == 100 ? match.Value : null;
        }

        public async Task<IReadOnlyList<string>> ScanAsync(Comic comic, IEnumerable<string> roots,
            CancellationToken token)
        {
            List<string> results = [];

            foreach (string root in roots)
            {
                token.ThrowIfCancellationRequested();

                // 1) Trouver l’auteur exact
                string? authorDir = FindExactAuthor(comic.Author, root);
                if (authorDir == null)
                    continue;

                // 2) Tous les comics de cet auteur
                var comics = Directory.EnumerateDirectories(authorDir, "*", SearchOption.AllDirectories);

                // 3) Chercher le comic exact
                string? exact = FindExactComic(comic.PackageName, comics);
                if (exact != null)
                {
                    if (comic.NumberPages > CountPages(exact))
                    {
                        results.Add(exact.Replace(root, "").TrimStart(Path.DirectorySeparatorChar));
                    }
                    else
                    {
                        // Comic trouvé mais pages insuffisantes
                        // Le ViewModel affichera un message via un event
                    }

                    continue;
                }

                // 4) Chercher chapitre précédent
                string? prev = FindPreviousChapter(comic.PackageName, root, comics);
                if (prev != null)
                {
                    results.Add(prev.Replace(root, "").TrimStart(Path.DirectorySeparatorChar));
                    continue;
                }
            }

            // 5) Fallback si rien trouvé
            if (results.Count == 0)
            {
                string fallback = BuildFallbackPath(comic);
                results.Add(fallback);
            }

            return results;
        }

        private static string BuildFallbackPath(Comic comic)
        {
            if (RegexUtility.ChapterRegex().IsMatch(comic.PackageName))
            {
                string comicName = RegexUtility.ChapterRegex()
                    .Replace(comic.PackageName, "")
                    .Trim();

                return Path.Combine(comic.Author, comicName, comic.PackageName);
            }

            return Path.Combine(comic.Author, comic.PackageName);
        }
    }
}
