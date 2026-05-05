using ComicsLib.Models;

namespace ComicsServiceLib.UI
{
    public interface ISettingsService
    {
        Options GetOptions();
        void SaveOptions();
    }
}
