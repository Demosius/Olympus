using Styx;
using Uranus;

namespace Pantheon.View;

/// <summary>
/// Interaction logic for ClanCreationWindow.xaml
/// </summary>
public partial class ClanCreationWindow
{
    public ClanCreationWindow(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon);
    }
}