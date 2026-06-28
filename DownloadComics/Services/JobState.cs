using ComicsInfraLib.Models.Views;
using ComicsServiceLib.UI;

namespace DownloadComics.Services
{
    public class JobState : IJobState
    {
        private SendViewModel? _viewModel;

        public void Init(SendViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void ClearState()
        {
            _viewModel?.States.Clear();
        }

        public void UpdateProgess(int progress, bool isDertmined)
        {
           _viewModel?.ProgressValue = progress;
           _viewModel?.IsDetermined = isDertmined;
        }

        public void UpdateState(string message, bool clear)
        {
            if (clear)
                ClearState();

            _viewModel?.States.Add(message);
        }

        public void UpdateTry(string stateJob)
        {
            _viewModel?.StateJob = stateJob;
        }
    }
}
