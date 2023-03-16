using Pantheon.ViewModels.PopUp.Rosters;
using Styx;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.Views.PopUp.Rosters;

/// <summary>
/// Interaction logic for RosterCreationWindow.xaml
/// </summary>
public partial class RosterCreationWindow
{
    public RosterCreationVM VM { get; set; }

    public RosterCreationWindow(Department department, Helios helios, Charon charon)
    {
        InitializeComponent();
        VM = new RosterCreationVM(department, helios, charon);
        DataContext = VM;
    }
}