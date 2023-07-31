using System;
using System.Windows.Controls;
using Morpheus.ViewModels.Controls.Inventory;
using Styx;
using Uranus;
using Uranus.Inventory.Models;

namespace Morpheus.Views.Controls.Inventory;

/// <summary>
/// Interaction logic for ZoneHandlerView.xaml
/// </summary>
public partial class ZoneHandlerView
{
    public ZoneHandlerVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Charon? Charon { get; set; }

    public ZoneHandlerView(Helios helios, Charon? charon)
    {
        Helios = helios;
        Charon = charon;
        InitializeComponent();
    }

    private async void ZoneHandlerView_OnInitialized(object? sender, EventArgs e)
    {
        VM = await ZoneHandlerVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }

    private void DataGrid_OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        var header = e.Column.Header.ToString();

        switch (header)
        {
            case nameof(NAVZone.Extension):
            case nameof(NAVZone.MoveLines):
            case nameof(NAVZone.NAVStock):
            case nameof(NAVZone.Bins):
            case nameof(NAVZone.Bays):
            case nameof(NAVZone.Location):
            case nameof(NAVZone.Stock):
            case nameof(NAVZone.Site):
                e.Cancel = true;
                break;
            case nameof(NAVZone.ID):
            case nameof(NAVZone.Code):
            case nameof(NAVZone.LocationCode):
            case nameof(NAVZone.SiteName):
                e.Column.IsReadOnly = true;
                break;
            case nameof(NAVZone.AccessLevel):
            case nameof(NAVZone.Description):
            case nameof(NAVZone.Ranking):
                break;
        }
    }
}