
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using ComicsInfraLib;
using ComicsInfraLib.Models.Views;
using ComicsInfraLib.Models.Views.Settings;
using ComicsInfraLib.Services;
using ComicsJDownloaderApi;
using ComicsLocalizationLib;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using ModernDownladComics.Models;
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

            AddViewModels(service);

            return service.BuildServiceProvider();
        }

        private static void AddViewModels(ServiceCollection service)
        {
            service.AddTransient<MainPageViewModel>();
            service.AddTransient<ArchivePageViewModel<XamlRoot>>();
            service.AddSingleton<ImportPageViewModel<XamlRoot>>();
            service.AddTransient<AddPageViewModel<XamlRoot>>();
            service.AddTransient<PathPageViewModel>();
            service.AddTransient<SettingsCredientialsPageViewModel<XamlRoot>>();
            service.AddTransient<SendViewModel>();
            service.AddTransient<SettingsAppPageViewModel<WindowId>>();
            service.AddTransient<SettingsHostPageViewModel<WindowId>>();
            service.AddTransient<AddLinksModelView>();
            service.AddTransient<ArchiveLinksViewModel>();
            service.AddTransient<ChangeSourcePageViewModel>();
        }

        private static void AddServices(ServiceCollection services)
        {
            services.AddSingleton(provider => new Lazy<Task<ComicsJDownloaderClient>>(() =>
               JDownloaderFactory.CreateAsnc(provider.GetRequiredService<ICredentialsService>()),
               LazyThreadSafetyMode.ExecutionAndPublication));
            services.AddSingleton<JdownloaderService>();
            services.AddSingleton<ListenerService>();
            services.AddSingleton<JDownloadJobService>();
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
            services.AddTransient<IDialogService<XamlRoot>, DialogService>();
            services.AddSingleton<ModernDownloadComicsLocalizationService>();
            services.AddSingleton<IPickerDialog<WindowId>, PickerDialog>();
            services.AddTransient<ContentPageService>();
            services.AddSingleton<ILocalizationService, ModernDownloadComicsLocalizationService>();
        }
    }
}
