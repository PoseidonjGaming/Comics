using ComicsInfraLib;
using ComicsInfraLib.Services;
using ComicsLib.Services;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using JDownloader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using ModernDownladComics.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;
        public static ServiceProvider? Services { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            ConfigureService();
            FileService.CreateFolder(FileService.BackupDirPath);
            FileService.CreateFolder(FileService.ComicsDir);

        }



        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            FileService.CurrentDir = AppContext.BaseDirectory;
            _window = new MainWindow();
            _window.Activate();
        }

        private static void ConfigureService()
        {
            ServiceCollection service = new();

            service.AddSingleton<ISettingsService, OptionService>();
            service.AddSingleton<ICredentialsService, CredentialsService>();
            service.AddSingleton<IHostService, HostSelectionService>();
            service.AddSingleton<IHtmlParserService, HtmlParserService>();
            service.AddSingleton<IWebService, WebService>();
            service.AddSingleton<IComicsBuilderService, ComicsBuilderService>();
            service.AddSingleton<IJobState, JobState>();
           
            service.AddSingleton(provider => new Lazy<Task<JDownloaderClient>>(()=>
                JDownloaderFactory.CreateAsnc(provider.GetRequiredService<ICredentialsService>()), 
                LazyThreadSafetyMode.ExecutionAndPublication));

            service.AddSingleton<JdownloaderService>();
            service.AddSingleton<ListenerService>();
            service.AddSingleton<JDownloadJobService>();

            Services = service.BuildServiceProvider();
        }
    }
}
