using Prometheus.ViewModels.Pages.Users;
using Styx;
using Uranus;

namespace Prometheus.Views.Pages.Users;

/// <summary>
/// Interaction logic for UserViewPage.xaml
/// </summary>
public partial class UserViewPage
{
    public UserViewPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        DataContext = new UserViewVM(helios, charon);
    }
}