using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib.UI;

namespace ComicsInfraLib.Services
{
    public class ArchiveService(IPathService pathService)
    {
        public IEnumerable<TreeItem> LoadTree()
        {
            foreach (var dir in Directory.GetDirectories(pathService.BackupDirPath))
                yield return BuildTree(dir);
        }

        private static TreeItem BuildTree(string dir)
        {
            TreeItem item = new(dir);

            var stack = new Stack<(TreeItem node, string path)>();
            stack.Push((item, dir));

            while (stack.Count > 0)
            {
                var (node, path) = stack.Pop();

                string[] subDirs;
                try
                {
                    subDirs = Directory.GetDirectories(path);
                }
                catch (Exception) { continue; }

                foreach (var subDir in subDirs)
                {
                    TreeItem child = new(subDir, node);
                    node.Children.Add(child);
                    stack.Push((child, subDir));
                }
            }

            return item;
        }

        public void RestoreBackup(TreeItem item)
        {
            string destPath = item.Path.Replace(pathService.BackupDirPath, FileUtility.ComicsDirectory);

            Directory.Move(destPath, item.Path);
        }

        public string GetAuthor(TreeItem item)
        {
            return item.Path.Replace(pathService.BackupDirPath, string.Empty)[1..]
                .Split(Path.DirectorySeparatorChar).First();
        }

        public void DeleteBackup(string path)
        {
            FileUtility.DeleteFolder(path);
        }
    }
}
