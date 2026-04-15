using DownloadComics.models;
using Newtonsoft.Json;
using System.IO;

namespace DownloadComics.services
{
    public class FileService
    {
        public static readonly string CurrentDir = Directory.GetCurrentDirectory();

        public static readonly string BackupDirPath = Path.Combine(CurrentDir, "data", "backup");
        public static readonly string ComicsDir = Path.Combine(CurrentDir, "data", "comics");

        public static readonly string DownloadPath = Path.Combine(CurrentDir, "data", "download.crawljob");
        public static readonly string AddedDownloadPath = Path.Combine(CurrentDir, "data", "added", "download.crawljob");
        public static readonly string BackupFilePath = Path.Combine(CurrentDir, "data", "backup.json");
        public static readonly string TrackFilePath = Path.Combine(CurrentDir, "data", "tracks.json");
        public const string ComicsDirectory = @"E:\Manga Scan\Manga\hentai";

        public static T? ReadFile<T>(string filePath)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
        }

        public static void WriteFile<T>(string filePath, T content) {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(content, Formatting.Indented));
        }

        public static void DeleteFile(string backupFilePath)
        {
            throw new NotImplementedException();
        }
    }
}
