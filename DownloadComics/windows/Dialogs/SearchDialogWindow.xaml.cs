using System.Windows;
using ComicsLib.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DownloadComics.windows.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour SearchDialogWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class SearchDialogWindow : Window
    {
        public TaskCompletionSource<DialogResult> Result { get; } = new();

        [ObservableProperty]
        public partial string MangaTXT { get; set; }

        [ObservableProperty]
        public partial string BackupTXT { get; set; }
        [ObservableProperty]
        public partial string JDTXT { get; set; }
        [ObservableProperty]
        public partial string Question { get; set; }
        public SearchDialogWindow(Window owner, string title, string question,
            string manga, string backup, string jd)
        {
            InitializeComponent();
            Owner = owner;

            DataContext = this;
            Title = title;
            Question = question;
            MangaTXT = manga;
            BackupTXT = backup;
            JDTXT = jd;
        }

        private void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            Result.SetResult(ComicsLib.Models.DialogResult.YES);
            DialogResult = true;
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            Result.SetResult(ComicsLib.Models.DialogResult.NO);
            DialogResult = false;
        }
    }
}
