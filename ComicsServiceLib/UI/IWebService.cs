using ComicsLib.Models;

namespace ComicsServiceLib.UI
{
    public interface IWebService
    {
        Task<string> Resolve(string url, RetrieveSource retrieve);
    }
}
