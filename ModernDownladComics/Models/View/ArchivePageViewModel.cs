using ComicsInfraLib.Services;
using ComicsLib.Utility;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FuzzierSharp.Extractor;
using SearchComicsLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModernDownladComics.Models.View
{
    public partial class ArchivePageViewModel(JdownloaderService jdownloaderService, IPathService pathService) : ObservableObject
    {
        public event Func<string?, IPathService, string, Task<bool>>? SearchDialogEvent;
        public event Func<Task<bool>>? RestoreDialogEvent;
        public ObservableCollection<TreeItem> Items { get; set; } = [];

        [ObservableProperty]
        public partial TreeItem? SelectedItem { get; set; }

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

        [RelayCommand]
        public async Task Search()
        {
            if (SelectedItem == null || SearchDialogEvent == null) return;

            string? jd = await jdownloaderService.GetComicJdownloader(SelectedItem.GetAuthor(),
                SelectedItem.Name);


            bool res = await SearchDialogEvent.Invoke(jd, pathService, SelectedItem.Name);
            if (res)
            {
                DeleteItem();
            }
        }

        public string AddToPanel(string path, string from)
        {
            if (SelectedItem == null)
                return "null";
            string authorPath = SearchUtility.GetAuthorPath(SelectedItem.GetAuthor(), path);

            IEnumerable<ExtractedResult<string>> results = SearchUtility.GetComics(authorPath,
                SelectedItem.Name);
            ExtractedResult<string>? res = results.FirstOrDefault();

            if (res == null)
                return "Empty";

            string fromPath = res.Value.Replace(authorPath, string.Empty)[1..];

            return $"From {from}: {SearchUtility.CountPage(res.Value)} pages - {fromPath}";
        }

        [RelayCommand]
        public void DeleteItem()
        {
            if (SelectedItem != null)
            {
                TreeItem item = SelectedItem;
                if (SelectedItem.Parent == null)
                {
                    Items.Remove(SelectedItem);
                    if (Directory.Exists(item.Path))
                        Directory.Delete(item.Path, true);
                }
                else
                {
                    TreeItem? itemRoot = SelectedItem.Remove();
                    if (itemRoot != null)
                    {
                        Items.Remove(itemRoot);
                        if (Directory.Exists(itemRoot.Path))
                            Directory.Delete(itemRoot.Path, true);
                    }

                }
            }
        }

        [RelayCommand]
        public async Task Restore()
        {
            if (SelectedItem != null && RestoreDialogEvent != null)
            {
                bool res = await RestoreDialogEvent.Invoke();
                if (res)
                {
                    string destPath = SelectedItem.Path.Replace(pathService.BackupDirPath,
                        FileUtility.ComicsDirectory);
                    if (Directory.Exists(destPath))
                        Directory.Delete(destPath, true);
                    Directory.Move(SelectedItem.Path, destPath);
                    DeleteItem();

                }
            }
        }

        public void Load()
        {
            foreach (var dir in Directory.GetDirectories(pathService.BackupDirPath))
            {
                Items.Add(BuildTree(dir));
            }
        }
    }
}
