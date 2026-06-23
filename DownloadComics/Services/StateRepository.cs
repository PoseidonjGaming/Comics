using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib.UI;
using System.Collections.ObjectModel;

namespace DownloadComics.Services
{
    public class StateRepository(IPathService pathService) : IStateRepository
    {
        public List<Track> Tracks { get; set; } = [];

        public ObservableCollection<Comic> Comics { get; set; } = [];

        public void SetComics(List<Comic> comics)
        {
            Comics.Clear();
            foreach (Comic comic in comics)
            {
                Comics.Add(comic);
            }
        }

        public void Save()
        {
            FileUtility.WriteFile(pathService.BackupFilePath, Comics);
            FileUtility.WriteFile(pathService.TrackFilePath, Tracks);
        }
    }
}
