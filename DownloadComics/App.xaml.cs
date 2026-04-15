using DownloadComics.resources;
using DownloadComics.resources.comic;
using DownloadComics.resources.common;
using DownloadComics.resources.credentialsSettings;
using DownloadComics.resources.main;
using DownloadComics.resources.priority;
using DownloadComics.resources.selectArchive;
using DownloadComics.resources.settings;
using DownloadComics.resources.restoreBuckup;
using System.Windows;
using DownloadComics.resources.path;
using DownloadComics.resources.changeSource;
using DownloadComics.windows;
using DownloadComics.resources.language;

namespace DownloadComics;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        string? language = DownloadComics.Properties.Settings.Default.Lang;

        TranslationSource.Instance.Load(string.IsNullOrEmpty(language) ? "en" : language,
            CommonStrings.ResourceManager, PriorityStrings.ResourceManager,
            MainStrings.ResourceManager, SelectArchiveStrings.ResourceManager,
            ComicStrings.ResourceManager, SettingsStrings.ResourceManager,
            CredentialsSettingsStrings.ResourceManager, PathStrings.ResourceManager,
           RestoreBackupStrings.ResourceManager, ChangeSourceStrings.ResourceManager,
          LanguageStrings.ResourceManager);
    }
}

