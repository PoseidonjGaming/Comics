using ComicsLib.Models;

namespace ComicsServiceLib
{
    public interface ISettingsService
    {
        Options GetOptions();
        void SaveOptions();
    }
}
