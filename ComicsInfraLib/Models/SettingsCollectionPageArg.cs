using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ComicsInfraLib.Models
{
    public record SettingsCollectionPageArg(ObservableCollection<string> List, bool IsHost);
}
