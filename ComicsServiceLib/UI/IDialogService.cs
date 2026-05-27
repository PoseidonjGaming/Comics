using ComicsLib.Models;

namespace ComicsServiceLib.UI
{
    public interface IDialogService<T> where T : class
    {
        Task<DialogResult> ShowSearchAsync(DialogArgs args, T arg);
        Task<DialogResult> ShowAddAsync(T arg);
        Task ShowErrorAsync(T arg, string msg);
        Task<DialogResult> ShowRestoreAsync(T arg);
    }
}
