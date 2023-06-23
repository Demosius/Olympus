using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Argos.ViewModels.Components;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;

namespace Argos.Views.Components;

/// <summary>
/// Interaction logic for CCNCommandView.xaml
/// </summary>
public partial class CCNCommandView
{
    public CCNCommandVM VM => (CCNCommandVM)DataContext;

    public CCNCommandView()
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
        VM.CheckLines();
    }
}