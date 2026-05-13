using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace ModernDownladComics.Models
{
    public partial class ComicInputModel : ObservableObject
    {
        [ObservableProperty]
        public partial string PackageName { get; set; }
        [ObservableProperty]
        public partial string Author { get; set; }
        [ObservableProperty]
        public partial string BaseURL { get; set; }
        [ObservableProperty]
        public partial int NumberPages { get; set; } = 0;

        public void Reset()
        {
            PackageName = string.Empty;
            Author = string.Empty;
            BaseURL = string.Empty;
            NumberPages = 0;
        }
    }
}
