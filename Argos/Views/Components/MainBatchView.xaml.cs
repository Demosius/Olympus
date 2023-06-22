using System.Linq;
using System.Windows.Controls;
using Argos.ViewModels.Components;

namespace Argos.Views.Components;

/// <summary>
/// Interaction logic for MainBatchView.xaml
/// </summary>
public partial class MainBatchView
{
    public MainBatchVM VM => (MainBatchVM)DataContext;

    public MainBatchView()
    {
        InitializeComponent();
    }

    private void BatchGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        VM.SelectedBatches.Clear();
        var batches = BatchGrid.SelectedItems
            .Cast<BatchVM>()
            .ToList();
        foreach (var batch in batches)
            VM.SelectedBatches.Add(batch);
        VM.CheckSums();
    }
}