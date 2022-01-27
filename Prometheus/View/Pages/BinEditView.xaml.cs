using Prometheus.ViewModel.Pages;
using Uranus.Inventory.Model;
using System.Windows;

namespace Prometheus.View.Pages
{
    /// <summary>
    /// Interaction logic for BinEditView.xaml
    /// </summary>
    public partial class BinEditView : Window
    {
        public BinEditView(BinVM parentVM, NAVBin bin = null)
        {
            InitializeComponent();

            var vm = DataContext as BinEditVM;
            vm!.Bin = bin;
            vm.ParentVM = parentVM;
        }
    }
}
