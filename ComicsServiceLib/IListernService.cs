using ComicsLib.Models;

namespace ComicsServiceLib
{
    public interface IListernService
    {
        Task StartListening(int port);
        Task<List<OfflineLink>> WaitJob();
        void StopListening();
    }
}
