using Sphynx.ViewModels;
using Styx;
using System.Threading.Tasks;
using Morpheus.ViewModels.Controls;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Sphynx.Views;

/// <summary>
/// Interaction logic for SphynxPage.xaml
/// </summary>
public partial class SphynxPage : IProject
{
    public SphynxVM VM { get; set; }

    public SphynxPage(Helios helios, Charon charon, ProgressBarVM progressBar)
    {
        VM =  new SphynxVM(helios, charon, progressBar);
        InitializeComponent();
        DataContext = VM;
    }

    public EProject Project => EProject.Sphynx;

    public static bool RequiresUser => false;

    public async Task RefreshDataAsync()
    {
        await VM.RefreshDataAsync();
    }
}