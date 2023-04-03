using Cadmus.ViewModels.Labels;

namespace Cadmus.Views.Labels;

/// <summary>
/// Interaction logic for ReceivingPutAwayLabelView.xaml
/// </summary>
public partial class ReceivingPutAwayLabelView
{
    public ReceivingPutAwayLabelView()
    {
        InitializeComponent();
    }

    public ReceivingPutAwayLabelView(ReceivingPutAwayLabelVM vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}