using Styx;
using Uranus;

namespace Pantheon.View;

/// <summary>
/// Interaction logic for RoleCreationWindow.xaml
/// </summary>
public partial class RoleCreationWindow
{
    public RoleCreationWindow(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon);
    }
}