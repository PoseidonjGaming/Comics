using ComicsLib.Models;
using ComicsLib.Utility;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModernDownladComics.windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ModernDownladComics.Models.View
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial Array Priorities { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<Comic> Comics { get; set; }

        [ObservableProperty]
        public partial int SelectedIndex { get; set; }

        [ObservableProperty]
        public partial Comic? SelectedComic { get; set; }

        private IPathService _pathService;

        public MainPageViewModel(IPathService pathService)
        {
            _pathService = pathService;
            Priorities = Enum.GetValues<Priority>();
            Comics = AppStateStore.Instance.Comics;
            SelectedComic = new();
        }

        [RelayCommand]
        private void Unselect()
        {
            SelectedComic = null;
        }

        [RelayCommand]
        private void Delete()
        {

            if (SelectedComic != null)
            {
                Comics.Remove(SelectedComic);

                FileUtility.WriteFile(_pathService.BackupFilePath, Comics);
            }
        }

        [RelayCommand]
        private void ChangeSource()
        {
            if (SelectedComic != null)
            {
                _ = new ChangeSourceWindow(SelectedComic);
            }
        }
    }
}
