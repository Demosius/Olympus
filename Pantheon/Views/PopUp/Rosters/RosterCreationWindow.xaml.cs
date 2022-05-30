using Styx;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.Views.PopUp.Rosters;

/// <summary>
/// Interaction logic for RosterCreationWindow.xaml
/// </summary>
public partial class RosterCreationWindow
{
    public RosterCreationWindow(Department department, Helios helios, Charon charon)
    {
        InitializeComponent();
        VM.SetDataSources(department, helios, charon);
    }
}