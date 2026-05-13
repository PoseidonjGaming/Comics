
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using ComicsInfraLib;
using ComicsInfraLib.Services;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using JDownloader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using ModernDownladComics.Models.View;
using ModernDownladComics.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModernDownladComics
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;
        public ServiceProvider Services { get; }

        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Services = ConfigureService();
        }



        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }

        private static ServiceProvider ConfigureService()
        {
            ServiceCollection service = new();

            AddServices(service);

            service.AddSingleton(provider => new Lazy<Task<JDownloaderClient>>(() =>
                JDownloaderFactory.CreateAsnc(provider.GetRequiredService<ICredentialsService>()),
                LazyThreadSafetyMode.ExecutionAndPublication));

            service.AddSingleton<JdownloaderService>();
            service.AddSingleton<ListenerService>();
            service.AddSingleton<JDownloadJobService>();

            AddViewModels(service);

            return service.BuildServiceProvider();
        }

        private static void AddViewModels(ServiceCollection service)
        {
            service.AddSingleton<MainPageViewModel>();
            service.AddTransient<ArchivePageViewModel>();
            service.AddTransient<ImportPageViewModel>();
            service.AddTransient<AddPageViewModel>();
            service.AddTransient<PathPageViewModel>();
        }

        private static void AddServices(ServiceCollection services)
        {
            services.AddSingleton<ISettingsService, OptionService>();
            services.AddSingleton<ICredentialsService, CredentialsService>();
            services.AddSingleton<IHostService, HostSelectionService>();
            services.AddSingleton<IHtmlParserService, HtmlParserService>();
            services.AddSingleton<IWebService, WebService>();
            services.AddSingleton<IComicsBuilderService, ComicsBuilderService>();
            services.AddSingleton<IJobState, JobState>();
            services.AddSingleton<IPathService, PathService>();
            services.AddSingleton<IStateRepository, StateRepository>();
            services.AddTransient<ArchiveService>();
            services.AddTransient<IScanService, ScanService>();
        }
    }
}
