using System.Threading.Tasks;
using Deimos.ViewModels;
using Morpheus.ViewModels.Controls;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Deimos.Views;

/// <summary>
/// Interaction logic for DeimosPage.xaml
/// </summary>
public partial class DeimosPage : IProject
{
    public DeimosVM VM { get; set; }

    public DeimosPage(Helios helios, ProgressBarVM progressBar)
    {
        VM = new DeimosVM(helios, progressBar);
        InitializeComponent();
        DataContext = VM;
    }

    public EProject Project => EProject.Deimos;

    public static bool RequiresUser => false;

    public async Task RefreshDataAsync()
    {
        await new Task(() => {});
    }
}