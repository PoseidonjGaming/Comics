using ComicsLib.Models;

namespace ComicsServiceLib
{
    public interface IComicsFilter
    {
        IEnumerable<Comic> Filter(IEnumerable<Comic> comics, string filter, string host);
    }
}
