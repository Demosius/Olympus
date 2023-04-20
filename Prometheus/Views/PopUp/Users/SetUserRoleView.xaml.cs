using Prometheus.ViewModels.Controls;
using Prometheus.ViewModels.PopUp.Users;
using Styx;
using Uranus;

namespace Prometheus.Views.PopUp.Users;

/// <summary>
/// Interaction logic for SetUserRoleView.xaml
/// </summary>
public partial class SetUserRoleView
{
    public SetUserRoleVM VM { get; set; }

    public SetUserRoleView(Helios helios, Charon charon, UserVM user)
    {
        VM = new SetUserRoleVM(helios, charon, user);
        InitializeComponent();
        DataContext = VM;
    }
}