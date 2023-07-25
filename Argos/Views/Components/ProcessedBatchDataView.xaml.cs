using System.Linq;
using System.Windows.Controls;
using Argos.ViewModels.Components;

namespace Argos.Views.Components;

/// <summary>
/// Interaction logic for ProcessedBatchDataView.xaml
/// </summary>
public partial class ProcessedBatchDataView
{
    public ProcessedBatchDataVM VM => (ProcessedBatchDataVM)DataContext;
    public ProcessedBatchDataView()
    {
        InitializeComponent();
    }

    private void CartonGroupGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        VM.SelectedGroups.Clear();
        var groups = CartonGroupGrid.SelectedItems
            .Cast<BatchTOGroupVM>()
            .ToList();
        foreach (var group in groups)
            VM.SelectedGroups.Add(group);
    }
}