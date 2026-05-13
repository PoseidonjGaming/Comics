using ComicsLib.Models;
using System.Collections.ObjectModel;

namespace ComicsServiceLib.UI
{
    public interface IStateRepository
    {
        ObservableCollection<Comic> Comics { get; }
        List<Track> Tracks { get; }

        void Save();
    }
}
