using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Cadmus.ViewModels.Controls;
using Cadmus.ViewModels.Labels;

namespace Cadmus.Views.Controls;

/// <summary>
/// Interaction logic for RefOrgeView.xaml
/// </summary>
public partial class RefOrgeDisplayView
{
    public RefOrgeDisplayView()
    {
        InitializeComponent();
        DataContext = new RefOrgeDisplayVM();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var vm = (RefOrgeDisplayVM)DataContext;
        vm.SelectedLabels =
            new ObservableCollection<RefOrgeLabelVM>(LabelViewList.SelectedItems
                .Cast<RefOrgeLabelVM>()
                .ToList());
    }
}
