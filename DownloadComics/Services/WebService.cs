using ComicsLib.Models;
using ComicsServiceLib.UI;
using DownloadComics.windows;

namespace DownloadComics.Services
{
    public class WebService : IWebService
    {
        public Task<string> Resolve(string url, RetrieveSource retrieve)
        {
            TaskCompletionSource<string> source = new();
            
            BrowserWindow browserWindow = new(new(url, retrieve, source));
            browserWindow.ShowDialog();
            return source.Task;
        }
    }
}
