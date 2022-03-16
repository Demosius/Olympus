using Styx;
using Uranus;

namespace Pantheon.View.Pages;

/// <summary>
/// Interaction logic for ShiftPage.xaml
/// </summary>
public partial class ShiftPage
{
    public ShiftPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon);
    }
}