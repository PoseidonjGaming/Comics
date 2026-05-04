using ComicsLib.Models;

namespace ComicsServiceLib
{
    public interface IWebService
    {
        Task<string> Resolve(string url, RetrieveSource retrieve);
    }
}
