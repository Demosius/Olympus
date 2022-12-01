using Prometheus.ViewModels.Helpers;
using Prometheus.ViewModels.Pages;
using Uranus;

namespace Prometheus.Views.Pages;

/// <summary>
/// Interaction logic for InventoryPage.xaml
/// </summary>
public partial class InventoryPage
{
    public InventoryPage(Helios helios)
    {
        InitializeComponent();
        DataContext = new InventoryVM(helios);
    }

    public override EDataCategory DataCategory => EDataCategory.Inventory;
}