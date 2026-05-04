using ComicsLib.Models;

namespace ComicsServiceLib
{
    public interface IHostService
    {
        string SelectHost(IEnumerable<Comic> comics, IEnumerable<string> hosts);
    }
}
