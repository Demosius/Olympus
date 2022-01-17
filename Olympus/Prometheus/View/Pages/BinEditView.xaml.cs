using Olympus.Prometheus.ViewModel.Pages;
using Uranus.Inventory.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Olympus.Prometheus.View.Pages
{
    /// <summary>
    /// Interaction logic for BinEditView.xaml
    /// </summary>
    public partial class BinEditView : Window
    {
        public BinEditView(BinVM parentVM, NAVBin bin = null)
        {
            InitializeComponent();

            BinEditVM vm = this.DataContext as BinEditVM;
            vm.Bin = bin;
            vm.ParentVM = parentVM;
        }
    }
}
