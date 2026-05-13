using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib.UI;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Services
{
    public class StateRepository(IPathService pathService) : IStateRepository
    {
        public ObservableCollection<Comic> Comics => AppStateStore.Instance.Comics;

        public List<Track> Tracks => AppStateStore.Instance.Tracks;


        public void Save()
        {
            FileUtility.WriteFile(pathService.BackupFilePath, Comics);
            FileUtility.WriteFile(pathService.TrackFilePath, Tracks);
        }
    }
}
