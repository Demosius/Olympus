using Prometheus.ViewModels.Helpers;
using Prometheus.ViewModels.Pages;
using Styx;
using Uranus;

namespace Prometheus.Views.Pages;

/// <summary>
/// Interaction logic for StaffPage.xaml
/// </summary>
public partial class StaffPage
{
    public StaffVM VM { get; set; }

    public StaffPage(Helios helios, Charon charon)
    {
        VM = new StaffVM(helios, charon);
        InitializeComponent();
        DataContext = VM;
    }

    public override EDataCategory DataCategory => EDataCategory.Staff;
}