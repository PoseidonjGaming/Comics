using ComicsInfraLib.Services;
using ComicsJDownloaderApi;
using ComicsJDownloaderApi.Model;
using ComicsLib.Models;
using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FuzzierSharp;
using FuzzierSharp.PreProcess;
using JDownloader.Model;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views
{
    public partial class MainPageViewModel(IStateRepository stateRepository): ObservableObject
    {
        public Array Priorities { get; } = Enum.GetValues<Priorities>();

        public ObservableCollection<Comic> Comics { get; set; } = 
            new(stateRepository.Comics);

        public event Action<Comic>? ChangeSourceRequested;

        [ObservableProperty]
        public partial Comic? SelectedComic { get; set; }

        [ObservableProperty]
        public partial string Filter { get; set; } = string.Empty;

        [ObservableProperty]
        public partial ObservableCollection<string> Hosts { get; set; } = [];

        [ObservableProperty]
        public partial string SelectedHost { get; set; } = "All";

        public void Init()
        {

            FilterComics();
            InitFilteredHosts();
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
                stateRepository.Comics.Remove(SelectedComic);
                Comics.Remove(SelectedComic);                
                stateRepository.Save();
            }
        }

        [RelayCommand]
        private async Task ChangeSource()
        {
            if (SelectedComic != null)
                ChangeSourceRequested?.Invoke(SelectedComic);
        }

        public void FilterComics()
        {
            Comics.Clear();

            var comics = stateRepository.Comics.Where(c =>
            {
                if (string.IsNullOrWhiteSpace(Filter) && SelectedHost == "All")
                    return true;


                bool filteredName = true;

                if (!string.IsNullOrEmpty(Filter))
                {
                    filteredName = Fuzz.TokenSetRatio(Filter, $"{c.PackageName} {c.Author}",
                     StandardPreprocessors.CaseInsensitive) >= 90;
                }


                bool filterHost = true;
                if (SelectedHost != "All")
                {
                    filterHost = c.Host == SelectedHost;
                }
                return filteredName && filterHost;
            }).ToList();

            foreach (var item in comics)
            {
                Comics.Add(item);
            }
        }

        private void InitFilteredHosts()
        {
            Hosts.Clear();
            foreach (var host in stateRepository.Comics.Select(c => c.Host).Distinct().Prepend("All"))
            {
                Hosts.Add(host);
            }
        }
    }
}
