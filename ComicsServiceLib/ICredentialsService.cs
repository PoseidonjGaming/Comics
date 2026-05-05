using ComicsLib.Models;

namespace ComicsServiceLib
{
    public interface ICredentialsService
    {
        JDCredentials GetCredentials();
        void SetCredentials(string email, string password, string device);
        void SaveCredentials();
    }
}
