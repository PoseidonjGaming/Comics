using ComicsLib.Models;

namespace ComicsServiceLib
{
    public interface IDownloadService
    {
        Task SendDownload(IEnumerable<Comic> comics);
    }
}
