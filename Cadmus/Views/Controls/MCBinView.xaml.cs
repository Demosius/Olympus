using System.Windows.Input;
using Cadmus.ViewModels.Controls;

namespace Cadmus.Views.Controls;

/// <summary>
/// Interaction logic for MCBinView.xaml
/// </summary>
public partial class MCBinView
{
    public MCBinView()
    {
        InitializeComponent();
    }

    private void ArrowButton_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        var vm = (MCBinVM) DataContext;
        vm.ShowStock = !vm.ShowStock;
    }
}