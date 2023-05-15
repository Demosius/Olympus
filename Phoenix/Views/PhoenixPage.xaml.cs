using System.Threading.Tasks;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Phoenix.Views;

/// <summary>
/// Interaction logic for TorchPage.xaml
/// </summary>
public partial class PhoenixPage : IProject
{
    public PhoenixPage()
    {
        InitializeComponent();
    }

    public EProject Project => EProject.Phoenix;

    public static bool RequiresUser => false;

    public async Task RefreshDataAsync()
    {
        await Task.Run(() => { });
    }
}