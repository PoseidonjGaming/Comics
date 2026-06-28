using System.Windows;
using ComicsInfraLib;
using ComicsInfraLib.Models.Views;
using ComicsInfraLib.Models.Views.Settings;
using ComicsInfraLib.Services;
using ComicsJDownloaderApi;
using ComicsLocalizationLib;
using ComicsServiceLib;
using ComicsServiceLib.UI;
using DownloadComics.Services;
using Microsoft.Extensions.DependencyInjection;

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
    }

    private static void AddModels(ServiceCollection services)
    {
        services.AddTransient<MainPageViewModel>();
        services.AddTransient<AddPageViewModel<Window>>();
        services.AddTransient<PathPageViewModel>();
        services.AddTransient<SettingsAppPageViewModel<Window>>();
        services.AddTransient<SettingsHostPageViewModel<Window>>();
        services.AddTransient<SettingsCredientialsPageViewModel<Window>>();
        services.AddTransient<ArchivePageViewModel<Window>>();
        services.AddTransient<ImportPageViewModel<Window>>();
        services.AddTransient<SendViewModel>();
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
        services.AddSingleton<IPathService, PathService>();
        services.AddTransient<IDialogService<Window>, DialogService>();
        services.AddTransient<ContentPageService>();
        services.AddSingleton<JdownloaderService>();
        services.AddSingleton<IHtmlParserService, HtmlParserService>();
        services.AddSingleton<IStateRepository, StateRepository>();
        services.AddSingleton<IJobState, JobState>();
        services.AddSingleton<IPickerDialog<Window>, PickerDialogService>();
        services.AddSingleton<IWebService, WebService>();
        services.AddSingleton<IComicsBuilderService, ComicsBuilderService>();
        services.AddSingleton<IHostService, HostSelectionService>();
        services.AddSingleton<IScanService, ScanService>();
        services.AddTransient<ArchiveService>();
        services.AddSingleton<JDownloadJobService>();
        services.AddSingleton<ILocalizationService, DownloadLocalizationService>();
    }
}

