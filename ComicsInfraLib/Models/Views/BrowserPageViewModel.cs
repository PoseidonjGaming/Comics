using ComicsLib.Models;
using System.Text.Json;

namespace ComicsInfraLib.Models.Views
{
    public class BrowserPageViewModel
    {
        public string URL { get; set; } = "";

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
                string js = "document.documentElement.outerHTML";
                string jsonResult = await getHtml(js);
                if (RetrieveSource == ComicsLib.Models.RetrieveSource.HTML)
                {
                    CompletionSource.TrySetResult(JsonSerializer.Deserialize<string>(jsonResult) ?? "empty");
                }
                else
                {
                    CompletionSource.TrySetResult(currentUrl);
                }

                return;
            }
            catch (Exception)
            {
            }
        }

        public void CleanUp() { 
            CompletionSource = null;
            RetrieveSource = null;
        }
    }
}
