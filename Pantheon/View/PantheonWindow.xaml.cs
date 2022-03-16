using Styx;
using Uranus;

namespace Pantheon.View;

/// <summary>
/// Interaction logic for PantheonWindow.xaml
/// </summary>
public partial class PantheonWindow
{
    public PantheonWindow(Charon charon, Helios helios)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon);
    }
}