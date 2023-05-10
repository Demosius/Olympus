using Panacea.ViewModels;
using System.Threading.Tasks;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Panacea.Views;

/// <summary>
/// Interaction logic for PanaceaPage.xaml
/// </summary>
public partial class PanaceaPage : IProject
{
    public PanaceaPage(Helios helios)
    {
        InitializeComponent();
        DataContext = new PanaceaVM(helios);
    }

    public EProject Project => EProject.Panacea;

    public static bool RequiresUser => false;

    public async Task RefreshDataAsync()
    {
        await new Task(() => { });
    }
}