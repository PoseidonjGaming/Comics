using ComicsServiceLib.UI;
using System.IO;

namespace DownloadComics.Services
{
    public class PathService : IPathService
    {
        private const string Data = "Data";
        public string BackupDirPath => Path.Combine(GetAppRoot(), Data, "Backup");

        public string ComicsDir => Path.Combine(GetAppRoot(), Data, "Comics");

        public string BackupFilePath => Path.Combine(GetAppRoot(), Data, "backup.json");

        public string TrackFilePath => Path.Combine(GetAppRoot(), Data, "tracks.json");

        public string GetAppRoot()
        {
            return Directory.GetCurrentDirectory();
        }

        public void MoveComic(string destPath, string sourcePath)
        {
            Directory.Move(sourcePath, destPath);
        }
    }
}
