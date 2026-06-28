using System.Collections.ObjectModel;

namespace ComicsInfraLib.Models
{
    public record SettingsCollectionPageArg<T>(ObservableCollection<string> List, T Arg,
        bool IsHost, string Entity);
}
