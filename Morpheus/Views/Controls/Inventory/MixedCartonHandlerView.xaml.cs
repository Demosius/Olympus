using Morpheus.ViewModels.Controls.Inventory;
using Uranus;

namespace Morpheus.Views.Controls.Inventory;

/// <summary>
/// Interaction logic for MixedCartonHandlerView.xaml
/// </summary>
public partial class MixedCartonHandlerView
{
    public MixedCartonHandlerView(Helios helios)
    {
        InitializeComponent();
        DataContext = new MixedCartonHandlerVM(helios);
    }
}