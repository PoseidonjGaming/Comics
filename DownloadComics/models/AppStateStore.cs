namespace DownloadComics.models
{
    public sealed class AppStateStore
    {
        private static readonly Lazy<AppState> _instance = new(() => new AppState());

        public static AppState Instance => _instance.Value;

        private AppStateStore() { }
    }
}
