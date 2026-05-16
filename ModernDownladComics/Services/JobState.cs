using ComicsInfraLib.Models.Views;
using ComicsServiceLib.UI;

namespace ModernDownladComics.Services
{
    public class JobState : IJobState
    {
        public SendViewModel? _viewModel;

        public void InitPage(SendViewModel page)
        {
            _viewModel = page;
        }

        
        public void UpdateState(string message, bool clear)
        {
            if (clear)
                ClearState();

            _viewModel?.States.Add(message);
        }
        public void ClearState()
        {
            _viewModel?.States.Clear();
        }

        public void UpdateTry(string stateJob)
        {
            _viewModel?.StateJob = stateJob;
        }

        public void UpdateProgess(int progress, bool IsDetermined)
        {
            _viewModel?.ProgressValue = progress;
            _viewModel?.IsDetermined = IsDetermined;                
        }

        
    }
}
