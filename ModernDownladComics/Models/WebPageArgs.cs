using ComicsLib.Models;
using System.Threading.Tasks;

namespace ModernDownladComics.Models
{
    public record WebPageArgs(string Url, RetrieveSource RetrieveSource, 
        TaskCompletionSource<string> Source);
}
