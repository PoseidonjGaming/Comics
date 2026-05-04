namespace ComicsServiceLib
{
    public interface IDialogService
    {
        Task ShowMessage(string title, string message);
    }
}
