using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib.UI;

namespace ComicsInfraLib.Services
{
    public class StateRepository(IPathService pathService) : IStateRepository
    {
        public List<Comic> Comics { get; set; } = [];
        public List<Track> Tracks { get; set; } = [];

        public void Save()
        {
            FileUtility.WriteFile(pathService.BackupFilePath, Comics);
            FileUtility.WriteFile(pathService.TrackFilePath, Tracks);
        }
    }
}
