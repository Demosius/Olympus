using System.Windows.Controls;
using Uranus.Inventory.Models;

namespace Hydra.Views.Controls;

/// <summary>
/// Interaction logic for ZoneHandlerView.xaml
/// </summary>
public partial class ZoneHandlerView
{
    public ZoneHandlerView()
    {
        InitializeComponent();
    }

    private void DataGrid_OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        var header = e.Column.Header.ToString();

        switch (header)
        {
            case nameof(NAVZone.Extension):
            case nameof(NAVZone.MoveLines):
            case nameof(NAVZone.Stock):
            case nameof(NAVZone.Bins):
            case nameof(NAVZone.Bays):
            case nameof(NAVZone.Location):
                e.Cancel = true;
                break;
            case nameof(NAVZone.Site):
            case nameof(NAVZone.ID):
            case nameof(NAVZone.Code):
            case nameof(NAVZone.LocationCode):
                e.Column.IsReadOnly = true;
                break;
            case nameof(NAVZone.AccessLevel):
            case nameof(NAVZone.Description):
            case nameof(NAVZone.Ranking):
                break;
        }
    }
}