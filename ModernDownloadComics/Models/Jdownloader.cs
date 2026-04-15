using FuzzierSharp;
using ModernDownloadComics.model;
using ModernDownloadComics.Services;

namespace ModernDownloadComics.Models
{
    public class Jdownloader
    {
        public const string ComicsDirectory = @"E:\Manga Scan\Manga\hentai";
        public string enabled { get; set; } = "FALSE";
        public string downloadFolder { get; set; } = "";
        public string autoConfirm { get; set; } = "TRUE";
        public string autoStart { get; set; } = "FALSE";
        public string extractAfterDownload { get => "TRUE"; }
        public string filename { get; set; } = "";
        public string text { get; set; } = "";
        public string packageName { get; set; } = "";
        public string comment { get; set; } = "";
        public string priority { get; set; } = Priority.DEFAULT.ToString();
        public bool deepAnalyseEnabled { get; set; } = true;

        public Jdownloader()
        {

        }


        public Jdownloader(Comic comic)
        {
            filename = comic.GetFilename();
            downloadFolder = comic.Path;
            text = comic.URL;
            packageName = comic.PackageName;
            comment = $"{comic.NumberPages} pages";
            enabled = comic.Enabled.ToString().ToUpper();
            priority = comic.Priority.ToString().ToUpper();
            IsConfirm(comic.Host);
        }

        public bool ShouldSerializefilename()
        {
            return filename.Split('.').Length > 1;
        }

        private void IsConfirm(string host)
        {
            Options? options = SettingsService.Get("options", new Options());
            if (options != null)
            {
                var result = Process.ExtractOne(host, options.Confirms);
                if (result != null && result.Score == 100) {
                    autoConfirm = "FALSE";
                }
            }
                
        }
    }
}
