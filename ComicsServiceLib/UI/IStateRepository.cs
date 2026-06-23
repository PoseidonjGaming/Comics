using ComicsLib.Models;
using System.Collections.ObjectModel;

namespace ComicsServiceLib.UI
{
    public interface IStateRepository
    {
        ObservableCollection<Comic> Comics { get; set; }
        List<Track> Tracks { get; set; }
        void SetComics(List<Comic> Comics);
        void Save();
    }
}
