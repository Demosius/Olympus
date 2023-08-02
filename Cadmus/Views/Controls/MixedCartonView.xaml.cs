using System.Windows.Controls;
using System.Windows.Input;
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
