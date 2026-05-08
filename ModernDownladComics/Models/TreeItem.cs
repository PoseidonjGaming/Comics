using ComicsServiceLib.UI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace ModernDownladComics.Models
{
    public class TreeItem(string path, TreeItem? parent = null)
    {
        public string Name = System.IO.Path.GetFileName(path);
        public string Path = path;
        public TreeItem? Parent = parent;
        public ObservableCollection<TreeItem> Children { get; set; } = [];

        public string GetAuthor()
        {
            var pathService = App.Current.Services.GetRequiredService<IPathService>();
            return Path.Replace(pathService.BackupDirPath, string.Empty)[1..]
                .Split(System.IO.Path.DirectorySeparatorChar).First();
        }

        public TreeItem? Remove()
        {
            if (Parent != null)
            {
                Parent.Children.Remove(this);
                if (Directory.Exists(Path))
                    Directory.Delete(Path, true);
                if (Parent.Children.Count == 0)
                {
                    return Parent.Remove();
                }
                else
                {
                    return this;
                }
            }
            else
            {
                return this;
            }

        }
    }
}
