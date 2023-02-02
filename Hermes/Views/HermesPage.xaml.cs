using Hermes.ViewModels;
using Styx;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Hermes.Views;

/// <summary>
/// Interaction logic for HermesPage.xaml
/// </summary>
public partial class HermesPage : IProject
{
    public HermesPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        DataContext = new HermesVM(helios, charon);
    }

    public EProject Project => EProject.Hermes;

    // Will use user data frequently, but need to allow those not signed in to send dev-request messages.
    //  Though should strongly recommend user log in.
    public static bool RequiresUser => false;

    public void RefreshData()
    {
        throw new System.NotImplementedException();
    }
}