using System.Threading.Tasks;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;
using Vulcan.ViewModels;

namespace Vulcan.Views;

/// <summary>
/// Interaction logic for VulcanPage.xaml
/// </summary>
public partial class VulcanPage : IProject
{
    public VulcanVM VM { get; set; }

    public VulcanPage(Helios helios)
    {
        VM = new VulcanVM(helios);
        InitializeComponent();
        DataContext = VM;
    }

    public EProject Project => EProject.Vulcan;

    public static bool RequiresUser => false;

    public async Task RefreshDataAsync() => await VM.RefreshDataAsync();
}