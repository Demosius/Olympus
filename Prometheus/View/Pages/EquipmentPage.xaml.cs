using Prometheus.ViewModel.Helpers;

namespace Prometheus.View.Pages;

/// <summary>
/// Interaction logic for EquipmentPage.xaml
/// </summary>
public partial class EquipmentPage : CatPage
{
    public EquipmentPage()
    {
        InitializeComponent();
    }

    public override EDataCategory DataCategory => EDataCategory.Equipment;
}