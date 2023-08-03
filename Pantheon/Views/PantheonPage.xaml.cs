using Styx;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Morpheus.ViewModels.Controls;
using Pantheon.ViewModels;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Pantheon.Views;

/// <summary>
/// Interaction logic for PantheonPage.xaml
/// </summary>
public partial class PantheonPage : IProject
{
    public PantheonVM VM { get; set; }

    public PantheonPage(Helios helios,Charon charon, ProgressBarVM progressBar)
    {
        InitializeComponent();
        VM = new PantheonVM(helios, charon, progressBar);
        DataContext = VM;
    }
    
    public EProject Project => EProject.Pantheon;

    public static bool RequiresUser => true;

    public async Task RefreshDataAsync()
    {
        await VM.RefreshPage();
    }

    private void Frame_OnNavigating(object sender, NavigatingCancelEventArgs e)
    {
        if (e.NavigationMode is NavigationMode.Back or NavigationMode.Forward) e.Cancel = true;
    }
}