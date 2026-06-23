namespace ComicsServiceLib.UI
{
    public interface IPickerDialog<T>
    {
        Task<string> FileOpenDialog(T arg, string title);
        Task<string> FolderOpenDialog(T arg, string title);
        Task<string> SaveFileDialog(T arg, string title);
    }
}
