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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cadmus.ViewModels.Controls;

namespace Cadmus.Views.Controls
{
    /// <summary>
    /// Interaction logic for MixedCartonView.xaml
    /// </summary>
    public partial class MixedCartonView : UserControl
    {
        public MixedCartonView()
        {
            InitializeComponent();
        }

        private void ArrowButton_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var vm = (MixedCartonVM) DataContext;
            vm.ShowBins = !vm.ShowBins;
        }
    }
}
