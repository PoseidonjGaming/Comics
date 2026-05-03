using ComicsLib.Models;

namespace ComicsLib.Services
{
    public interface ICredentialsService
    {
        JDCredentials GetCredentials();
        void SetCredentials(string email, string password, string device);
        void SaveCredentials();
    }
}
