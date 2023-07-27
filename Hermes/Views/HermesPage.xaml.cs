using System.Threading.Tasks;
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
    public HermesVM VM { get; set; }

    public HermesPage(Helios helios, Charon charon)
    {
        VM = new HermesVM(helios, charon);
        InitializeComponent();
        DataContext = VM;
    }

    public EProject Project => EProject.Hermes;

    // Will use user data frequently, but need to allow those not signed in to send dev-request messages.
    //  Though should strongly recommend user log in.
    public static bool RequiresUser => false;

    public async Task RefreshDataAsync()
    {
        await VM.RefreshDataAsync();
    }
}