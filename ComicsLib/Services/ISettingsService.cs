using ComicsLib.Models;

namespace ComicsLib.Services
{
    public interface ISettingsService
    {
        Options GetOptions();
        void SaveOptions();
    }
}
