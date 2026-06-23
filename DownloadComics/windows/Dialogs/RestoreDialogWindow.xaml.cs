using ComicsLib.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace DownloadComics.windows.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour RestoreDialogWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class RestoreDialogWindow : Window
    {
        public TaskCompletionSource<DialogResult> Result { get; } = new();
        
        [ObservableProperty]
        public partial string Question { get; set; }
        public RestoreDialogWindow(string content, string comic)
        {
            InitializeComponent();
            Question = string.Format(content, comic);
            DataContext = this;
        }

        private void ConfirmBTN_Click(object sender, RoutedEventArgs e)
        {
            Result.SetResult(ComicsLib.Models.DialogResult.YES);
            DialogResult = true;
        }

        private void Deny_Click(object sender, RoutedEventArgs e)
        {
            Result.SetResult(ComicsLib.Models.DialogResult.NO);
            DialogResult = true;
        }
    }
}
