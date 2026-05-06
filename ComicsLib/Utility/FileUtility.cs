using Newtonsoft.Json;

namespace ComicsLib.Utility
{
    public class FileUtility
    {
        public const string ComicsDirectory = @"E:\Manga Scan\Manga\hentai";
        public static T? ReadFile<T>(string filePath)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
        }

        public static void WriteFile<T>(string filePath, T content)
        {
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
