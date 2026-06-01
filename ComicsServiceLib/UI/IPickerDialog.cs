namespace ComicsServiceLib.UI
{
    public interface IPickerDialog<T>
    {
        Task<string> FileOpenDialog(T arg, string title);
        Task<string> FolderDialog(T arg, string title);
    }
}
