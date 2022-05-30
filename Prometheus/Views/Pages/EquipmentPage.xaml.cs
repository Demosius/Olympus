using Prometheus.ViewModels.Helpers;

namespace Prometheus.Views.Pages;

/// <summary>
/// Interaction logic for EquipmentPage.xaml
/// </summary>
public partial class EquipmentPage
{
    public EquipmentPage()
    {
        InitializeComponent();
    }

    public override EDataCategory DataCategory => EDataCategory.Equipment;
}