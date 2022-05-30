using Prometheus.ViewModels.Helpers;

namespace Prometheus.Views.Pages;

/// <summary>
/// Interaction logic for InventoryPage.xaml
/// </summary>
public partial class InventoryPage
{
    public InventoryPage()
    {
        InitializeComponent();
    }

    public override EDataCategory DataCategory => EDataCategory.Inventory;
}