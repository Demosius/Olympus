using Styx;
using Uranus;

namespace Prometheus.View.Pages.Users;

/// <summary>
/// Interaction logic for UserViewPage.xaml
/// </summary>
public partial class UserViewPage
{
    public UserViewPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon);
    }
}