using FuzzierSharp;
using FuzzierSharp.Extractor;

namespace SearchComicsLib
{
    public static class SearchUtility
    {
        public static void BrowseFolder(string path, string root)
        {
            foreach (var folder in Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly))
            {
                string relative = folder.StartsWith(root) && folder.Length > root.Length
                    ? folder[root.Length..].TrimStart(Path.DirectorySeparatorChar)
                    : folder;
                bool hasSubDirs = false;
                try
                {
                    using var enumerator = Directory.EnumerateDirectories(folder).GetEnumerator();
                    hasSubDirs = enumerator.MoveNext();
                }
                catch { /* Ignore inaccessible folders */ }

                if (hasSubDirs)
                {
                    BrowseFolder(folder, root);
                }
                else
                {
                    Console.WriteLine($"{relative}: {CountPage(folder)} pages");
                }


            }
        }

        public static int CountPage(string path)
        {
            int pages = 0;
            var dirs = new Stack<string>();
            dirs.Push(path);

            while (dirs.Count > 0)
            {
                var current = dirs.Pop();
                try
                {
                    foreach (var _ in Directory.EnumerateFiles(current))
                        pages++;

                    foreach (var dir in Directory.GetDirectories(current))
                        dirs.Push(dir);
                }
                catch { /* Ignore inaccessible folders */ }
            }
            return pages;
        }

        public static string GetAuthorPath(string author, string root)
        {
            var result = Process.ExtractOne(author,
                Directory.EnumerateDirectories(root, "*", SearchOption.TopDirectoryOnly),
                s => Path.GetFileNameWithoutExtension(s).ToLower());
            return result != null ? result.Value : root;
        }

        public static IEnumerable<ExtractedResult<string>> GetComics(string authorPath, string comic)
        {
            IEnumerable<string> dirs = Directory.EnumerateDirectories(authorPath, "*",
                SearchOption.AllDirectories);

            return Process.ExtractSorted(comic, dirs, s =>
            Path.GetFileNameWithoutExtension(s).ToLower().Replace("\\W", ""), cutoff: 100);
        }
    }
}
