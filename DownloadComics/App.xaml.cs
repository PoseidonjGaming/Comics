using ComicsInfraLib;
using ComicsInfraLib.Models.Views;
using ComicsInfraLib.Models.Views.Settings;
using ComicsInfraLib.Services;
using ComicsJDownloaderApi;
using ComicsLocalizationLib;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using DownloadComics.Services;
using DownloadComics.windows;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DownloadComics;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IServiceProvider ServiceProvider;
    public new static App Current => (App)Application.Current;
    public App()
    {
        ServiceCollection services = new();
        AddServices(services);
        AddModels(services);

        ServiceProvider = services.BuildServiceProvider();

        var settingsService= ServiceProvider.GetRequiredService<ISettingsService>();
        ServiceProvider.GetRequiredService<DownloadLocalizationService>().LoadLang(settingsService.GetOptions().Lang);
    }

    private static void AddModels(ServiceCollection services)
    {
        services.AddTransient<MainPageViewModel>();
        services.AddTransient<AddPageViewModel<Window, DownloadLocalizationService>>();
        services.AddTransient<PathPageViewModel>();
        services.AddTransient<SettingsAppPageViewModel<Window, DownloadLocalizationService>>();
        services.AddTransient<SettingsHostPageViewModel<Window, DownloadLocalizationService>>();
        services.AddTransient<SettingsCredientialsPageViewModel<Window, DownloadLocalizationService>>();
        services.AddTransient<ArchivePageViewModel<Window, DownloadLocalizationService>>();
        services.AddTransient<ImportPageViewModel<Window, DownloadLocalizationService>>();
        services.AddTransient<SendViewModel<DownloadLocalizationService>>();
        services.AddTransient<ChangeSourcePageViewModel>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        if (ServiceProvider is IDisposable disposable)
            disposable.Dispose();
    }

    private static void AddServices(ServiceCollection services)
    {
        services.AddSingleton(provider => new Lazy<Task<ComicsJDownloaderClient>>(() =>
               JDownloaderFactory.CreateAsnc(provider.GetRequiredService<ICredentialsService>()),
               LazyThreadSafetyMode.ExecutionAndPublication));

        services.AddSingleton<ICredentialsService, CredentialsService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<DownloadLocalizationService>();
        services.AddSingleton<IPathService, PathService>();
        services.AddTransient<IDialogService<Window>, DialogService>();
        services.AddTransient<ContentPageService<DownloadLocalizationService>>();
        services.AddSingleton<JdownloaderService<DownloadLocalizationService>>();
        services.AddSingleton<IHtmlParserService, HtmlParserService>();
        services.AddSingleton<IStateRepository, StateRepository>();
        services.AddSingleton<IJobState, JobState>();
        services.AddSingleton<IPickerDialog<Window>, PickerDialogService>();
        services.AddSingleton<IWebService, WebService>();
        services.AddSingleton<IComicsBuilderService, ComicsBuilderService>();
        services.AddSingleton<IHostService, HostSelectionService>();
        services.AddSingleton<IScanService, ScanService>();
        services.AddTransient<ArchiveService>();
        services.AddSingleton<JDownloadJobService<DownloadLocalizationService>>();
    }
}

