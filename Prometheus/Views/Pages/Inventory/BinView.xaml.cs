using System;
using Prometheus.ViewModels.Helpers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Prometheus.ViewModels.Pages.Inventory;
using Styx;
using Uranus;

namespace Prometheus.Views.Pages.Inventory;

/// <summary>
/// Interaction logic for BinView.xaml
/// </summary>
public partial class BinView
{
    public BinVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public BinView(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        InitializeComponent();
    }


    private async void BinView_OnInitialized(object? sender, EventArgs e)
    {
        VM = await BinVM.CreateAsync(Helios, Charon);
        DataContext = VM;
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