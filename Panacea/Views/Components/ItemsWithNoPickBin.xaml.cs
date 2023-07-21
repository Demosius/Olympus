using System.Windows.Controls;

namespace Panacea.Views.Components;

/// <summary>
/// Interaction logic for ItemsWithNoPickBin.xaml
/// </summary>
public partial class ItemsWithNoPickBin
{
    public ItemsWithNoPickBin()
    {
        InitializeComponent();
    }

    private void DataGrid_OnLoadingRow(object? sender, DataGridRowEventArgs e)
    {
        e.Row.Header = (e.Row.GetIndex()+1).ToString();
    }
}