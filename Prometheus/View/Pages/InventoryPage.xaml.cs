using Prometheus.ViewModel.Helpers;

namespace Prometheus.View.Pages;

/// <summary>
/// Interaction logic for InventoryPage.xaml
/// </summary>
public partial class InventoryPage : CatPage
{
    public InventoryPage()
    {
        InitializeComponent();
    }

    public override EDataCategory DataCategory => EDataCategory.Inventory;
}