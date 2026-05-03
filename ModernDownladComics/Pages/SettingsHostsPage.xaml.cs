using ComicsLib.Models;
using ComicsLib.Utilities;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ModernDownladComics.Models;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernDownladComics.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsHostsPage : Page
{
    public ObservableCollection<string> Collection { get; set; }
    public ObservableString Value { get; set; }
    public string SelectedHost { get; set; }
    private bool isHost;
    public SettingsHostsPage()
    {
        InitializeComponent();
        Collection = [];
        Value = new(string.Empty);
        SelectedHost = string.Empty;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if(e.Parameter is SettingsPageArgs<ObservableCollection<string>> settings)
        {
            Collection = settings.Arg;
            isHost = settings.IsHost;
        }
    }

    private void AddHostBTN_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (isHost){
            Collection.Add(RegexUtility.HostRegex().Match(Value.Value).Value);
        }
        else
        {
            Collection.Add(Value.Value);
        }
       
        Value.Value = string.Empty;
    }

    private void DeleteHostBTN_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(SelectedHost))
        {
            Collection.Remove(SelectedHost);
        }
    }
}
