using ComicsServiceLib.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ModernDownladComics.Services
{
    public class PathService : IPathService
    {
        protected const string Data = "Data";

        public string BackupDirPath => Path.Combine(GetAppRoot(), Data, "Backup");

        public string ComicsDir => Path.Combine(GetAppRoot(), Data, "Comics");

        public string BackupFilePath => Path.Combine(GetAppRoot(), Data, "backup.json");

        public string TrackFilePath => Path.Combine(GetAppRoot(), Data, "tracks.json");

        public string GetAppRoot()
        {
            return Path.GetDirectoryName(Environment.ProcessPath) ?? "";
        }
    }
}
