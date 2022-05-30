using Styx;
using Uranus;
using Uranus.Users.Models;

namespace Prometheus.Views.PopUp.Users;

/// <summary>
/// Interaction logic for SetUserRoleView.xaml
/// </summary>
public partial class SetUserRoleView
{
    public SetUserRoleView(Helios helios, Charon charon, User user)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon, user);
    }
}