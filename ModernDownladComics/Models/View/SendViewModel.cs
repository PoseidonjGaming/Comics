using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ModernDownladComics.Models.View
{
    public partial class SendViewModel(string job, string stateJob) : ObservableObject
    {
        [ObservableProperty]
        public partial string? Job { get; set; } = job;
        [ObservableProperty]
        public partial string? StateJob { get; set; } = stateJob;
        [ObservableProperty]
        public partial int ProgessValue { get; set; }
        [ObservableProperty]
        public partial bool IsDetermined { get; set; } = true;

        public ObservableCollection<string> States { get; set; } = [];
    }
}
