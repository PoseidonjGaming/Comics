using ComicsLib.Models;

namespace ComicsInfraLib.Models
{
    public record WebPageArgs(string Url, RetrieveSource RetrieveSource, 
        TaskCompletionSource<string> Source);
}
