using System.Collections.ObjectModel;

namespace ComicsServiceLib.UI
{
    public interface IJobState
    {
        void UpdateState(string message, bool clear);
        void ClearState();
        void UpdateTry(string tr);
        void UpdateProgess(int progress, bool isDertmined);
    }
}
