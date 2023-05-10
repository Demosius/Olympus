using Hydra.ViewModels;
using Styx;
using System.Threading.Tasks;
using System.Windows;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Hydra.Views;

/// <summary>
/// Interaction logic for HydraPage.xaml
/// </summary>
public partial class HydraPage : IProject
{
    public HydraPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        DataContext = new HydraVM(helios, charon);
    }

    public EProject Project => EProject.Hydra;

    public static bool RequiresUser => false;

    public async Task RefreshDataAsync()
    {
        await new Task(() => {});
    }

    private void ActionToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        ZoneToggle.IsChecked = false;
        SiteToggle.IsChecked = false;
        LevelsToggle.IsChecked = false;
    }

    private void ZoneToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        ActionToggle.IsChecked = false;
        SiteToggle.IsChecked = false;
        LevelsToggle.IsChecked = false;
    }

    private void SiteToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        ZoneToggle.IsChecked = false;
        ActionToggle.IsChecked = false;
        LevelsToggle.IsChecked = false;
    }

    private void LevelsToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        ActionToggle.IsChecked = false;
        SiteToggle.IsChecked = false;
        ZoneToggle.IsChecked = false;
    }
}