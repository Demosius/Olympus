using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Cadmus.ViewModels.Controls;
using Cadmus.ViewModels.Labels;

namespace Cadmus.Views.Controls;

/// <summary>
/// Interaction logic for CCNDisplayView.xaml
/// </summary>
public partial class CCNDisplayView
{
    public CCNDisplayVM VM { get; set; }

    public CCNDisplayView()
    {
        VM = new CCNDisplayVM();
        InitializeComponent();
        DataContext = VM;
    }

    private void LabelViewList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        VM.SelectedLabels =
            new ObservableCollection<CartonLabelVM>(LabelViewList.SelectedItems
                .Cast<CartonLabelVM>()
                .ToList());
    }

    private void LabelGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        VM.SelectedGridLabels =
            new ObservableCollection<CartonLabelVM>(LabelGrid.SelectedItems
                .Cast<CartonLabelVM>()
                .ToList());
    }
}