using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views
{
    public partial class ArchivePageViewModel<T>(JdownloaderService jdownloaderService,
        IPathService pathService, ArchiveService archiveService,
        IDialogService<T> dialogService) : BaseLocViewModel where T : class
    {
        public ObservableCollection<TreeItem> Items { get; } = [];

        public TreeItem SelectedItem { get; set; } = new TreeItem("");

        public Dictionary<string, string> Loc = [];

        [RelayCommand]
        public async Task Search(T arg)
        {
            if (SelectedItem == null) return;

            string? jd = await jdownloaderService.GetComicJdownloader(archiveService.GetAuthor(SelectedItem),
                SelectedItem.Name);


            DialogResult res = await dialogService.ShowSearchAsync(new(SelectedItem.Name,
                archiveService.GetAuthor(SelectedItem), pathService.BackupDirPath, jd), arg);
            if (res == DialogResult.SUCCESS)
            {
                DeleteItem();
            }
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
                    archiveService.DeleteBackup(item.Path);
                }
                else
                {
                    TreeItem? itemRoot = SelectedItem.Remove();
                    if (itemRoot != null)
                    {
                        Items.Remove(itemRoot);
                        archiveService.DeleteBackup(itemRoot.Path);
                    }

                }
            }
        }

        [RelayCommand]
        public async Task Restore(T arg)
        {
            if (SelectedItem != null)
            {
                DialogResult res = await dialogService.ShowRestoreAsync(arg);
                if (res == DialogResult.SUCCESS)
                {
                    archiveService.RestoreBackup(SelectedItem);
                    DeleteItem();

                }
            }
        }

        public void Load()
        {
            Items.Clear();
            foreach (var item in archiveService.LoadTree())
            {
                Items.Add(item);
            }
        }
    }
}