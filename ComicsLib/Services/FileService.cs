using Newtonsoft.Json;

namespace ComicsLib.Services
{
    public class FileService
    {
        private const string Data = "Data";
        public const string ComicsDirectory = @"E:\Manga Scan\Manga\hentai";

        public static string CurrentDir = Path.GetDirectoryName(Environment.ProcessPath) ?? "";

        public static readonly string BackupDirPath = Path.Combine(CurrentDir, Data, "Backup");
        
        public static readonly string ComicsDir = Path.Combine(CurrentDir, Data, "Comics");

        public static readonly string BackupFilePath = Path.Combine(CurrentDir, Data, "backup.json");
        public static readonly string TrackFilePath = Path.Combine(CurrentDir, Data, "tracks.json");
       

        public static T? ReadFile<T>(string filePath)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
        }

        public static void WriteFile<T>(string filePath, T content) {
            CreateFolder(Path.GetDirectoryName(filePath) ?? "");
            File.WriteAllText(filePath, JsonConvert.SerializeObject(content, Formatting.Indented));
        }

        public static void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
