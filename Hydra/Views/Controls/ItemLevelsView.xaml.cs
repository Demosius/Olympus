using Hydra.ViewModels.Controls;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hydra.Views.Controls;

/// <summary>
/// Interaction logic for ItemLevelsView.xaml
/// </summary>
public partial class ItemLevelsView
{
    public ItemLevelsView()
    {
        InitializeComponent();
    }

    private void DataGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        var addedItems = e.AddedCells;
        if (!addedItems.Any()) return;
        var c = addedItems[0].Column;
        ((ItemLevelsVM)DataContext).SelectedObject = ((DataRowView)addedItems[0].Item).Row.ItemArray[c.DisplayIndex];
    }

    private void DataGrid_OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        var col = e.Column;

        if (col.Header.ToString() != "Item")
        {
            ((DataGridTextColumn)col).ElementStyle = new Style(typeof(TextBlock))
            {
                Setters =
                {
                    new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center)
                }
            };
        }
    }
}