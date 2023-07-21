using Styx;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Prometheus.ViewModels;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Prometheus.Views;

/// <summary>
/// Interaction logic for Prometheus.xaml
/// </summary>
public partial class PrometheusPage : IProject
{
    public PrometheusVM VM { get; set; }

    public PrometheusPage(Helios helios, Charon charon)
    {
        VM = new PrometheusVM(helios, charon);
        InitializeComponent();
        DataContext = VM;
    }

    public EProject Project => EProject.Prometheus;

    public static bool RequiresUser => false;

    public async Task RefreshDataAsync()
    {
        await Task.Run(() => {});
    }

    private void Frame_OnNavigating(object sender, NavigatingCancelEventArgs e)
    {
        if (e.NavigationMode is NavigationMode.Back or NavigationMode.Forward) e.Cancel = true;
    }
}