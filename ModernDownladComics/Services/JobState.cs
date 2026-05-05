using ComicsLib.Models;
using ComicsServiceLib.UI;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Pages;
using System;

namespace ModernDownladComics.Services
{
    public class JobState : IJobState
    {
        public SendPage? _page;
        private TextBlock? _textBlock;
        private ProgressBar? _progressBar;

        public void InitPage(SendPage page, TextBlock textBlock, ProgressBar progressBar)
        {
            _page = page;
            _textBlock = textBlock;
            _progressBar = progressBar;
        }

        
        public void UpdateState(string message, bool clear)
        {
            if (clear)
                ClearState();

            _page?.DispatcherQueue.TryEnqueue(() => _page.States.Add(message));
        }
        public void ClearState()
        {
            _page?.States.Clear();
        }

        public void UpdateTry(string tr)
        {
            if (_textBlock != null)
                _textBlock.Text = tr;
        }

        public void UpdateProgess(int progress, bool IsDetermined)
        {
            if(_progressBar != null)
            {
                _progressBar.Value = progress;
                _progressBar.IsIndeterminate = IsDetermined;
                _progressBar.Maximum = AppStateStore.Instance.Comics.Count;
            }
                
        }

        
    }
}
