namespace ComicsLib.Models
{
    public abstract class AppStateStore
    {
        private static readonly Lazy<AppState> _instance = new(() => new AppState());

        public static AppState Instance => _instance.Value;

        private AppStateStore() { }
    }
}
