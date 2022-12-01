using Prometheus.ViewModels.Helpers;
using Prometheus.ViewModels.Pages;
using Styx;
using System.Windows.Navigation;
using Uranus;

namespace Prometheus.Views.Pages;

/// <summary>
/// Interaction logic for UserPage.xaml
/// </summary>
public partial class UserPage
{
    public UserPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        DataContext = new UserVM(helios, charon);
    }

    public override EDataCategory DataCategory => EDataCategory.Users;

    private void Frame_OnNavigating(object sender, NavigatingCancelEventArgs e)
    {
        if (e.NavigationMode is NavigationMode.Back or NavigationMode.Forward)
            e.Cancel = true;
    }
}