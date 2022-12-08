using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Cadmus.ViewModels;
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

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var vm = (ReceivingPutAwayVM)DataContext;
        vm.SelectedLabels =
            new ObservableCollection<ReceivingPutAwayLabelVM>(LabelViewList.SelectedItems
                .Cast<ReceivingPutAwayLabelVM>()
                .ToList());
    }
}