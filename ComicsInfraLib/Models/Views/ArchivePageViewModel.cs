using ComicsInfraLib.Services;
using ComicsLib.Models;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views
{
    public partial class ArchivePageViewModel(JdownloaderService jdownloaderService,
        IPathService pathService, ArchiveService archiveService) : ObservableObject
    {
        public event Func<DialogArgs, Task<bool>>? SearchDialogEvent;
        public event Func<Task<bool>>? RestoreDialogEvent;
        public ObservableCollection<TreeItem> Items { get; } = [];

        public TreeItem SelectedItem { get; set; } = new TreeItem("");

        [RelayCommand]
        public async Task Search()
        {
            if (SelectedItem == null || SearchDialogEvent == null) return;

            string? jd = await jdownloaderService.GetComicJdownloader(archiveService.GetAuthor(SelectedItem),
                SelectedItem.Name);


            bool res = await SearchDialogEvent.Invoke(new(SelectedItem.Name,
                archiveService.GetAuthor(SelectedItem), pathService.BackupDirPath, jd));
            if (res)
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
        public async Task Restore()
        {
            if (SelectedItem != null && RestoreDialogEvent != null)
            {
                bool res = await RestoreDialogEvent.Invoke();
                if (res)
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