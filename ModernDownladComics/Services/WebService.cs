using ComicsLib.Models;
using ComicsServiceLib;
using Microsoft.UI.Xaml.Controls;
using ModernDownladComics.Models;
using ModernDownladComics.Pages;
using System;
using System.Threading.Tasks;

namespace ModernDownladComics.Services
{
    public class WebService : IWebService
    {
        private Frame? frame ;

        public void Init(Frame frame)
        {
            this.frame = frame;
        }
        public Task<string> Resolve(string url, RetrieveSource retrieve)
        {
            TaskCompletionSource<string> source = new();
            frame?.Navigate(typeof(BrowserPage),
                    new WebPageArgs(url, retrieve, source));
            return source.Task;
        }
    }
}
