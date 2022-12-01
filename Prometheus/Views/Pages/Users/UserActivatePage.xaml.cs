using Prometheus.ViewModels.Pages.Users;
using Styx;
using Uranus;

namespace Prometheus.Views.Pages.Users;

/// <summary>
/// Interaction logic for UserActivatePage.xaml
/// </summary>
public partial class UserActivatePage
{
    public UserActivatePage(Helios helios, Charon charon)
    {
        InitializeComponent();
        DataContext = new UserActivateVM(helios, charon);
    }
}