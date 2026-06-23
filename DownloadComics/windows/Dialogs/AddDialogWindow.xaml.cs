using System.Windows;
using ComicsLib.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DownloadComics.windows.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour AddDialogWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class AddDialogWindow : Window
    {
        public TaskCompletionSource<DialogResult> Result = new();

        [ObservableProperty]
        public partial string Text { get; set; }
        public AddDialogWindow(Window owner, string title, string content)
        {
            InitializeComponent();
            Owner = owner;

            Title = title;
            Text = content;
            DataContext = this;
        }

        private void AddBTN_Click(object sender, RoutedEventArgs e)
        {
            Result.SetResult(ComicsLib.Models.DialogResult.YES);
            DialogResult = true;
        }

        private void NoBTN_Click(object sender, RoutedEventArgs e)
        {
            Result.SetResult(ComicsLib.Models.DialogResult.NO);
            DialogResult = true;
        }

        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            Result.SetResult(ComicsLib.Models.DialogResult.CANCELLED);
            DialogResult = true;
        }
    }
}
