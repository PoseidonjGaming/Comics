using System.Collections.ObjectModel;

namespace ComicsLib.Models
{
    public class TreeItem(string path, TreeItem? parent = null)
    {
        public string Name = System.IO.Path.GetFileName(path);
        public string Path = path;
        public TreeItem? Parent = parent;
        public ObservableCollection<TreeItem> Children { get; set; } = [];

        

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
