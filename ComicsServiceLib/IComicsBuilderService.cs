using ComicsLib.Models;

namespace ComicsServiceLib
{
    public interface IComicsBuilderService
    {
        Task<Comic?> MakeComics(string baseUrl, string author, string name, int numberPage,
            bool isScan, string? htmlBody = "");
    }
}
