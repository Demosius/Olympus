using Prometheus.ViewModel.Pages.Inventory;
using Uranus.Inventory.Model;

namespace Prometheus.View.Pages.Inventory;

/// <summary>
/// Interaction logic for BinEditView.xaml
/// </summary>
public partial class BinEditView
{
    public BinEditView(BinVM parentVM, NAVBin? bin = null)
    {
        InitializeComponent();

        var vm = DataContext as BinEditVM;
        vm!.Bin = bin;
        vm.ParentVM = parentVM;
    }
}