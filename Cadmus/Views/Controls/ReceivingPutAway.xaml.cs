using Cadmus.ViewModels.Controls;

namespace Cadmus.Views.Controls;

/// <summary>
/// Interaction logic for ReceivingPutAway.xaml
/// </summary>
public partial class ReceivingPutAway
{
    public ReceivingPutAway()
    {
        InitializeComponent();
        DataContext = new ReceivingPutAwayVM();
    }
}