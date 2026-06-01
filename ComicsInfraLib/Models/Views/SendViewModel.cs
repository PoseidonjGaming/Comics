using ComicsServiceLib.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models.Views
{
    public partial class SendViewModel(IStateRepository stateRepository) : ObservableObject
    {
        [ObservableProperty]
        public partial string Job { get; set; } = "Start";
        [ObservableProperty]
        public partial string StateJob { get; set; } = "Job not started";
        [ObservableProperty]
        public partial int ProgressValue { get; set; }
        [ObservableProperty]
        public partial bool IsDetermined { get; set; } = true;
        public int Maximum { get; set; } = stateRepository.Comics.Count;

        public ObservableCollection<string> States { get; } = [];
    }
}
