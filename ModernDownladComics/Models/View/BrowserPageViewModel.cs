using ComicsLib.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ModernDownladComics.Models.View
{
    public partial class BrowserPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string? URL { get; set; }

        private RetrieveSource? RetrieveSource { get; set; }
        private TaskCompletionSource<string>? CompletionSource { get; set; }


        public void Init(WebPageArgs args)
        {
            URL = args.Url;
            RetrieveSource = args.RetrieveSource;
            CompletionSource = args.Source;

        }

        public async Task OnNavigationCompleted(Func<string, Task<string>> getHtml, string currentUrl)
        {
            if (CompletionSource == null || RetrieveSource == null) return;

            try
            {
                if (RetrieveSource == ComicsLib.Models.RetrieveSource.HTML)
                {
                    string js = "document.documentElement.outerHTML";
                    string jsonResult = await getHtml(js);
                    CompletionSource.TrySetResult(JsonSerializer.Deserialize<string>(jsonResult) ?? "empty");
                }
                else
                {
                    CompletionSource.TrySetResult(currentUrl);
                }


            }
            catch (Exception)
            {
            }
        }

        public void Cleanup()
        {
            CompletionSource = null;
            RetrieveSource = null;
        }
    }
}
