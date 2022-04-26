using Styx;
using Uranus;

namespace Prometheus.View.Pages.Users;

/// <summary>
/// Interaction logic for UserDeactivatePage.xaml
/// </summary>
public partial class UserDeactivatePage
{
    public UserDeactivatePage(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon);
    }
}