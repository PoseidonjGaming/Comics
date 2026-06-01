using ComicsLib.Models;

namespace ComicsServiceLib
{
    public interface IListernService
    {
        Task StartListening(int port);
        Task WaitJob();
        void StopListening();
    }
}
