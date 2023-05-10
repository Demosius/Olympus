using System.Threading.Tasks;
using Cadmus.ViewModels;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Cadmus.Views;

/// <summary>
/// Interaction logic for CadmusPage.xaml
/// </summary>
public partial class CadmusPage : IProject
{
    public CadmusPage(Helios helios)
    {
        InitializeComponent();
        DataContext = new CadmusVM(helios);
    }

    public EProject Project => EProject.Cadmus;

    public static bool RequiresUser => false;

    public async Task RefreshDataAsync()
    {
        await new Task(() => {});
    }
}