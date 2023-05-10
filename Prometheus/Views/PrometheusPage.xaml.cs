using Styx;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Prometheus.Views;

/// <summary>
/// Interaction logic for Prometheus.xaml
/// </summary>
public partial class PrometheusPage : IProject
{
    public PrometheusPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon);
    }

    public EProject Project => EProject.Prometheus;

    public static bool RequiresUser => false;

    public async Task RefreshDataAsync()
    {
        await new Task(() => {});
    }

    private void Frame_OnNavigating(object sender, NavigatingCancelEventArgs e)
    {
        if (e.NavigationMode is NavigationMode.Back or NavigationMode.Forward) e.Cancel = true;
    }
}