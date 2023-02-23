using Morpheus.ViewModels.Controls;
using Uranus;

namespace Morpheus.Views.Controls;

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