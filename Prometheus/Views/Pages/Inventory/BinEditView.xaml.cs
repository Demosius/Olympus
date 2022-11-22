using Prometheus.ViewModels.Pages.Inventory;
using Uranus.Inventory.Models;

namespace Prometheus.Views.Pages.Inventory;

/// <summary>
/// Interaction logic for BinEditView.xaml
/// </summary>
public partial class BinEditView
{
    public BinEditView(BinVM parentVM, NAVBin? bin = null)
    {
        InitializeComponent();

        VM!.Bin = bin;
        VM.ParentVM = parentVM;
    }
}