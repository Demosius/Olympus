using Styx;
using Uranus;
using Uranus.Staff;

namespace Aion.View;

/// <summary>
/// Interaction logic for ManagerView.xaml
/// </summary>
public partial class AionPage : IProject
{
    public AionPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon);
    }

    public EProject Project => EProject.Aion;

    public static bool RequiresUser => true;

    public void RefreshData()
    {
        VM.RefreshData();
    }
}