using Prometheus.ViewModels.Helpers;

namespace Prometheus.Views.Pages.Inventory;

/// <summary>
/// Interaction logic for BatchView.xaml
/// </summary>
public partial class BatchView
{
    public BatchView()
    {
        InitializeComponent();
    }

    public override EDataType DataType => EDataType.Batch;
}