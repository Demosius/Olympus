using Prometheus.ViewModel.Helpers;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Prometheus.View.Pages.Inventory;

/// <summary>
/// Interaction logic for BinView.xaml
/// </summary>
public partial class BinView
{
    public BinView()
    {
        InitializeComponent();
    }

    public override EDataType DataType => EDataType.NAVBin;

    private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        var headerName = e.Column.Header.ToString() ?? "";

        //Cancel the column you don't want to generate
        if (new List<string> { "ID", "ZoneID", "Zone", "NAVStock", "Stock", "Extension", "Dimensions", "Bay", "AccessLevel" }.Contains(headerName))
        {
            e.Cancel = true;
        }

        e.Column.Header = headerName switch
        {
            //update column details when generating
            "LocationCode" => "Location",
            "ZoneCode" => "Zone",
            "UsedCube" => "Used Cube",
            "MaxCube" => "Max Cube",
            "LastCCDate" => "Last Cycle Count Date",
            "LastPIDate" => "Last Physical Inventory Date",
            _ => e.Column.Header
        };
    }
}