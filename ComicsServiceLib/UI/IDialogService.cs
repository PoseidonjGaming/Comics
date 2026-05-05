namespace ComicsServiceLib.UI
{
    public interface IDialogService
    {
        Task ShowMessage(string title, string message);
    }
}
