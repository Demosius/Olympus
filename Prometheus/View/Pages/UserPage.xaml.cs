using System.Windows.Navigation;
using Prometheus.ViewModel.Helpers;
using Styx;
using Uranus;

namespace Prometheus.View.Pages;

/// <summary>
/// Interaction logic for UserPage.xaml
/// </summary>
public partial class UserPage
{
    public UserPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon);
    }

    public override EDataCategory DataCategory => EDataCategory.Users;

    private void Frame_OnNavigating(object sender, NavigatingCancelEventArgs e)
    {
        if (e.NavigationMode is NavigationMode.Back or NavigationMode.Forward)
            e.Cancel = true;
    }
}