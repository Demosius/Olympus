using Styx;
using Uranus;

namespace Pantheon.View.Pages;

/// <summary>
/// Interaction logic for RosterPage.xaml
/// </summary>
public partial class RosterPage
{
    public RosterPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon);
    }
}