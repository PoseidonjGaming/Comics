using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ComicsServiceLib.UI
{
    public interface IPathService
    {


        public string BackupDirPath { get; }
        public string ComicsDir { get; }

        public string BackupFilePath { get; }
        public string TrackFilePath { get; }
        string GetAppRoot();
    }
}
