using CommunityToolkit.Mvvm.ComponentModel;

namespace ComicsInfraLib.Models.Views
{
    public abstract class BaseLocViewModel : ObservableObject
    {
        public Dictionary<string, string> Loc { get; set; } = [];

        public void InitData(Dictionary<string, string> data)
        {
            Loc = data;
        }
    }
}
