using ComicsLib.Models;

namespace ComicsServiceLib
{
    public interface IScanService
    {
        Task<IReadOnlyList<string>> ScanAsync(Comic comic,
            IEnumerable<string> roots, CancellationToken token);

        int CountPages(string path);
        string? FindExactComic(string packageName, IEnumerable<string> directories);
        string? FindPreviousChapter(string packageName, string root, IEnumerable<string> directories);
    }
}
