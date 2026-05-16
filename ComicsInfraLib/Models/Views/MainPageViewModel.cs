using ComicsLib.Models;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views
{
    public partial class MainPageViewModel(IStateRepository stateRepository) : 
        ObservableObject
    {
        public Array Priorities { get; } = Enum.GetValues<Priorities>();

        public ObservableCollection<Comic> Comics { get; } = AppStateStore.Instance.Comics;

        public event Action<Comic>? ChangeSourceRequested;


        [ObservableProperty]
        public partial Comic? SelectedComic { get; set; }

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
                stateRepository.Save();
            }
        }

        [RelayCommand]
        private void ChangeSource()
        {
            if (SelectedComic != null)
                ChangeSourceRequested?.Invoke(SelectedComic);
        }
    }
}
