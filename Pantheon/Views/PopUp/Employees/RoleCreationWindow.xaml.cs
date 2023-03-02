using Pantheon.ViewModels.PopUp.Employees;
using Styx;
using Uranus;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for RoleCreationWindow.xaml
/// </summary>
public partial class RoleCreationWindow
{
    public RoleCreationVM VM { get; set; }

    public RoleCreationWindow(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM = new RoleCreationVM(helios, charon);
        DataContext = VM;
    }
}