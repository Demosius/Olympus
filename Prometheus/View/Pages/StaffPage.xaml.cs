using Prometheus.ViewModel.Helpers;

namespace Prometheus.View.Pages;

/// <summary>
/// Interaction logic for StaffPage.xaml
/// </summary>
public partial class StaffPage : CatPage
{
    public StaffPage()
    {
        InitializeComponent();
    }

    public override EDataCategory DataCategory => EDataCategory.Staff;
}