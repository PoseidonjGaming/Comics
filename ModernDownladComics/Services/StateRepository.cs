using System.Collections.Generic;
using System.Collections.ObjectModel;
using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib.UI;

namespace ModernDownladComics.Services
{
    public class StateRepository(IPathService pathService): IStateRepository
    {
        public ObservableCollection<Comic> Comics { get; set; } = [];
        public List<Track> Tracks { get; set; } = [];

        public void Save()
        {
            FileUtility.WriteFile(pathService.BackupFilePath, Comics);
            FileUtility.WriteFile(pathService.TrackFilePath, Tracks);
        }

        public void SetComics(List<Comic> comics)
        {
            Comics.Clear();
            foreach (Comic comic in comics)
            {
                Comics.Add(comic);
            }
        }
    }
}
