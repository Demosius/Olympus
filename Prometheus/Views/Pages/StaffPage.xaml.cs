using Prometheus.ViewModels.Helpers;

namespace Prometheus.Views.Pages;

/// <summary>
/// Interaction logic for StaffPage.xaml
/// </summary>
public partial class StaffPage
{
    public StaffPage()
    {
        InitializeComponent();
    }

    public override EDataCategory DataCategory => EDataCategory.Staff;
}