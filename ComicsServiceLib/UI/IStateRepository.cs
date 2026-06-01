using ComicsLib.Models;
using System.Collections.ObjectModel;

namespace ComicsServiceLib.UI
{
    public interface IStateRepository
    {
        List<Comic> Comics { get; set; }
        List<Track> Tracks { get; set; }

        void Save();
    }
}
