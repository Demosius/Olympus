using Hydra.ViewModels.PopUps;

namespace Hydra.Views.PopUps;

/// <summary>
/// Interaction logic for ItemSelectionWindow.xaml
/// </summary>
public partial class ItemSelectionWindow
{
    public ItemSelectionWindow(ItemSelectionVM vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}