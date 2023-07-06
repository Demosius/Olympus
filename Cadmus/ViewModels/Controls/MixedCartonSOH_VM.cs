using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Cadmus.Annotations;
using Cadmus.Interfaces;
using Cadmus.ViewModels.Commands;
using Microsoft.Win32;
using Morpheus.Helpers;
using Morpheus.ViewModels.Controls;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Uranus;
using Uranus.Commands;
using Uranus.Extensions;
using Uranus.Interfaces;
using Uranus.Inventory;
using Uranus.Inventory.Models;
using Settings = Cadmus.Properties.Settings;

namespace Cadmus.ViewModels.Controls;

public class MixedCartonSOH_VM : INotifyPropertyChanged, IDBInteraction, IFilters, IExport
{
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public List<MixedCartonVM> AllMixedCartons { get; set; }

    public bool CanExport => MixedCartons.Any();

    #region INotifyPropertyChanged Members

    public ObservableCollection<MixedCartonVM> MixedCartons { get; set; }

    private bool autoDetect;
    public bool AutoDetect
    {
        get => autoDetect;
        set
        {
            autoDetect = value;
            OnPropertyChanged();
        }
    }

    public string ZoneString
    {
        get => Settings.Default.MCZoneString;
        set
        {
            Settings.Default.MCZoneString = value;
            OnPropertyChanged();
            Settings.Default.Save();
        }
    }

    public string LocationString
    {
        get => Settings.Default.MCLocationString;
        set
        {
            Settings.Default.MCLocationString = value;
            OnPropertyChanged();
            Settings.Default.Save();
        }
    }

    private string zoneFilter;
    public string ZoneFilter
    {
        get => zoneFilter;
        set
        {
            zoneFilter = value;
            OnPropertyChanged();
        }
    }

    private string platformFilter;
    public string PlatformFilter
    {
        get => platformFilter;
        set
        {
            platformFilter = value;
            OnPropertyChanged();
        }
    }

    private string categoryFilter;
    public string CategoryFilter
    {
        get => categoryFilter;
        set
        {
            categoryFilter = value;
            OnPropertyChanged();
        }
    }

    private string divisionFilter;
    public string DivisionFilter
    {
        get => divisionFilter;
        set
        {
            divisionFilter = value;
            OnPropertyChanged();
        }
    }

    private string totalDisplay;
    public string TotalDisplay
    {
        get => totalDisplay;
        set
        {
            totalDisplay = value;
            OnPropertyChanged();
        }
    }

    private string filteredDisplay;
    public string FilteredDisplay
    {
        get => filteredDisplay;
        set
        {
            filteredDisplay = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ExportToCSVCommand ExportToCSVCommand { get; set; }
    public ExportToExcelCommand ExportToExcelCommand { get; set; }
    public ExportToLabelsCommand ExportToLabelsCommand { get; set; }
    public ExportToPDFCommand ExportToPDFCommand { get; set; }

    #endregion

    public MixedCartonSOH_VM(Helios helios, ProgressBarVM progressBar)
    {
        Helios = helios;
        ProgressBar = progressBar;

        zoneFilter = string.Empty;
        platformFilter = string.Empty;
        divisionFilter = string.Empty;
        categoryFilter = string.Empty;
        totalDisplay = string.Empty;
        filteredDisplay = string.Empty;

        AllMixedCartons = new List<MixedCartonVM>();
        MixedCartons = new ObservableCollection<MixedCartonVM>();

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ExportToCSVCommand = new ExportToCSVCommand(this);
        ExportToExcelCommand = new ExportToExcelCommand(this);
        ExportToLabelsCommand = new ExportToLabelsCommand(this);
        ExportToPDFCommand = new ExportToPDFCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        ProgressBar.StartTask("Pulling Mixed Carton Data...");

        var (mixedCartons, stockDataSet, stockNotes) =
            await Helios.InventoryReader.MixedCartonStockAsync(ZoneString.Split('|'), LocationString.Split('|'));

        var mcTool = new MixedCartonIdentificationTool(mixedCartons);
        var mcBins = stockDataSet.Bins.Values.Where(bin => bin.MixedCartonStockConversion(mcTool, AutoDetect)).ToList();

        AllMixedCartons = mcBins.SelectMany(b => b.MixedCartons).Distinct().Select(mc => new MixedCartonVM(mc, Helios, stockNotes.ToDictionary(sn => sn.ID, sn => sn)))
            .ToList();

        SetTotalDisplay();

        ApplyFilters();

        ProgressBar.EndTask();
    }

    public void ClearFilters()
    {
        ZoneFilter = string.Empty;
        PlatformFilter = string.Empty;
        DivisionFilter = string.Empty;
        CategoryFilter = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        var mcList = AllMixedCartons.Where(mc =>
            mc.FilterZones(ZoneFilter) > 0 &&
            Regex.IsMatch(mc.Platform, PlatformFilter, RegexOptions.IgnoreCase) &&
            Regex.IsMatch(mc.Category, CategoryFilter, RegexOptions.IgnoreCase) &&
            Regex.IsMatch(mc.Division, DivisionFilter, RegexOptions.IgnoreCase));

        MixedCartons.Clear();

        foreach (var mc in mcList)
            MixedCartons.Add(mc);

        SetFilteredDisplay();
    }

    public void SetTotalDisplay() => TotalDisplay = GetDisplay(AllMixedCartons);

    public void SetFilteredDisplay() => FilteredDisplay = GetDisplay(MixedCartons);

    public static string GetDisplay(IEnumerable<MixedCartonVM> mixedCartonVMs)
    {
        var mcList = mixedCartonVMs.ToList();
        var mcTypes = mcList.Count;
        var locations = mcList.Sum(mc => mc.Bins.Count);
        var cartons = mcList.Sum(mc => mc.Cases);

        return $"{mcTypes} Mixed Cartons -- {locations} Bin Locations -- {cartons} Total Cases";
    }

    public Task ExportToPDF()
    {
        MessageBox.Show("Cannot Export to PDF at this time.", "No", MessageBoxButton.OK, MessageBoxImage.Information);
        return Task.CompletedTask;
    }

    public async Task ExportToCSV()
    {
        await Task.Run(() =>
        {
            Output.DataTableToCSV(GetDataTable(), "MixedCarton_SoH.csv");
        });
    }

    public async Task ExportToExcel()
    {
        // Generate recommended file name.
        var dir = Settings.Default.MixedCartonSoHSaveDirectory;
        var fileName = "MixedCarton_SoH.xlsx";

        // Confirm save location with saveFileDialog window.
        var saveDialog = new SaveFileDialog
        {
            InitialDirectory = dir,
            FileName = fileName,
            DefaultExt = "xls"
        };

        var result = saveDialog.ShowDialog();

        if (result != true) return;

        // Get file name and save stored settings accordingly.
        var path = saveDialog.FileName;
        dir = Path.GetDirectoryName(path);
        Settings.Default.MixedCartonSoHSaveDirectory = dir;
        Settings.Default.Save();

        if (File.Exists(path))
        {
            var mbResult = MessageBox.Show(
                "File Exists: Would you like to update existing file?\n\n(No - Overwrite file.)",
                "File Update", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            switch (mbResult)
            {
                case MessageBoxResult.No:
                    await Task.Run(() => CreateExcelFile(path));
                    break;
                case MessageBoxResult.Yes:
                    await Task.Run(() => EditExcelFile(path));
                    break;
                case MessageBoxResult.None:
                case MessageBoxResult.OK:
                case MessageBoxResult.Cancel:
                default:
                    return;
            }
        }
        else
            await Task.Run(() => CreateExcelFile(path));


        fileName = Path.GetFileName(path);
        MessageBox.Show($"Successfully exported {fileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
    }

    public Task ExportToLabels()
    {
        MessageBox.Show("Cannot Export to Labels at this time.", "No", MessageBoxButton.OK, MessageBoxImage.Information);
        return Task.CompletedTask;
    }

    private DataTable GetDataTable()
    {
        var dt = new DataTable();

        dt.Columns.Add(new DataColumn("RACK LOCATION"));
        dt.Columns.Add(new DataColumn("SKU"));
        dt.Columns.Add(new DataColumn("Item Name"));
        dt.Columns.Add(new DataColumn("Design"));
        dt.Columns.Add(new DataColumn("QTY IN THIS LOCATION"));
        dt.Columns.Add(new DataColumn("SHIPPABLE PACK"));
        dt.Columns.Add(new DataColumn("QTY PER PACK"));
        dt.Columns.Add(new DataColumn(" "));
        dt.Columns.Add(new DataColumn("OTHER NOTES"));

        foreach (var mixedCartonVM in MixedCartons)
        {
            var totalLines = mixedCartonVM.Bins.Sum(b => b.Stock.Count);
            var mid = (int)Math.Ceiling(totalLines / 2.0);
            var line = 0;
            var qtyNote = mixedCartonVM.Bins.Count > 1
                ? $"{mixedCartonVM.Cases} PACKS IN TOTAL"
                : "THIS IS ALL MIXED PACKS ON HAND";
            foreach (var mcBinVM in mixedCartonVM.Bins)
            {
                foreach (var mcStockVM in mcBinVM.Stock)
                {
                    line++;
                    var row = dt.NewRow();

                    row["RACK LOCATION"] = mcBinVM.Code;
                    row["SKU"] = mcStockVM.Sku;
                    row["Item Name"] = mcStockVM.ItemName;
                    row["Design"] = mixedCartonVM.Name;
                    row["QTY IN THIS LOCATION"] = mcStockVM.LocationQty;
                    row["SHIPPABLE PACK"] = "YES";
                    row["QTY PER PACK"] = mcStockVM.QtyPerCarton;
                    if (line == mid) row[" "] = qtyNote;
                    row["OTHER NOTES"] = mcStockVM.Note;

                    dt.Rows.Add(row);
                }
            }
        }

        return dt;
    }

    private void CreateExcelFile(string path)
    {
        // Create workbook and sheet.
        using var package = new ExcelPackage();

        var workbook = package.Workbook;
        var sheet = workbook.Worksheets.Add("MIX PACKS");
        sheet.DefaultRowHeight = 18;

        // Create headers.
        sheet.Cells[1, 1].Value = "RACK LOCATION";
        sheet.Cells[1, 2].Value = "SKU";
        sheet.Cells[1, 3].Value = "Item Name";
        sheet.Cells[1, 4].Value = "Design";
        sheet.Cells[1, 5].Value = "QTY IN THIS LOCATION";
        sheet.Cells[1, 6].Value = "SHIPPABLE PACK";
        sheet.Cells[1, 7].Value = "QTY PER PACK";
        sheet.Cells[1, 8].Value = "";
        sheet.Cells[1, 9].Value = "OTHER NOTES";

        // Format Headers
        using (var range = sheet.Cells[1, 1, 1, 9])
        {
            range.Style.Font.Bold = true;
            range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Style.Font.Size = 12;
        }

        for (var col = 1; col <= 9; col++)
            sheet.Cells[1, col].Style.Border.BorderAround(ExcelBorderStyle.Medium);

        // Fill remaining data
        var row = 1;
        foreach (var mixedCartonVM in MixedCartons)
        {
            var totalLines = mixedCartonVM.Bins.Sum(b => b.Stock.Count);
            var mid = (int)Math.Ceiling(totalLines / 2.0);
            var line = 0;
            var qtyNote = mixedCartonVM.Bins.Count > 1
                ? $"{mixedCartonVM.Cases} PACKS IN TOTAL"
                : "THIS IS ALL MIXED PACKS ON HAND";
            var startRow = row + 1;
            foreach (var mcBinVM in mixedCartonVM.Bins)
            {
                foreach (var mcStockVM in mcBinVM.Stock)
                {
                    line++;
                    row++;

                    sheet.Cells[row, 1].Value = mcBinVM.Code;
                    sheet.Cells[row, 2].Value = mcStockVM.Sku;
                    sheet.Cells[row, 3].Value = mcStockVM.ItemName;
                    sheet.Cells[row, 4].Value = mixedCartonVM.Name;
                    sheet.Cells[row, 5].Value = mcStockVM.LocationQty;
                    sheet.Cells[row, 6].Value = "YES";
                    sheet.Cells[row, 7].Value = mcStockVM.QtyPerCarton;
                    if (line == mid) sheet.Cells[row, 8].Value = qtyNote;
                    sheet.Cells[row, 9].Value = mcStockVM.Note;

                    for (var col = 1; col <= 9; col++)
                        sheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
            }

            // Format MC Area
            using var range = sheet.Cells[startRow, 1, row, 9];
            range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Style.Font.Size = 11;
        }

        for (var i = 1; i <= 9; i++)
            sheet.Column(i).AutoFit();

        var xlFile = new FileInfo(path);

        package.SaveAs(xlFile);
    }

    private void EditExcelFile(string path)
    {
        var xlFile = new FileInfo(path);

        // Set workbook and sheet.
        using var package = new ExcelPackage(xlFile);
        var workbook = package.Workbook;
        MixedPackIndices col;

        // Find sheet.
        var sheet = workbook.Worksheets.FirstOrDefault(s => MixedPackIndices.HasHeaders(s.GetHeaderColumns()));

        var newSheet = sheet is null;

        if (sheet is not null)
        {
            col = new MixedPackIndices(sheet.GetHeaderColumns());
            // Increment each by one, to match excel cells.
            col.IncrementColumns();
            col.SetBlank();
        }
        else
        {
            col = new MixedPackIndices(new[]
            {
                "gap", "RACK LOCATION", "SKU", "Item Name", "Design", "QTY IN THIS LOCATION", "SHIPPABLE PACK",
                "QTY PER PACK", "",
                "OTHER NOTES"
            });

            var sheetName = "MIX PACKS";
            var no = 0;
            while (workbook.Worksheets.Any(s => s.Name == sheetName))
            {
                no++;
                sheetName = $"MIX PACKS {no:00}";
            }

            sheet = workbook.Worksheets.Add(sheetName);

            // Create headers.
            sheet.Cells[1, 1].Value = "RACK LOCATION";
            sheet.Cells[1, 2].Value = "SKU";
            sheet.Cells[1, 3].Value = "Item Name";
            sheet.Cells[1, 4].Value = "Design";
            sheet.Cells[1, 5].Value = "QTY IN THIS LOCATION";
            sheet.Cells[1, 6].Value = "SHIPPABLE PACK";
            sheet.Cells[1, 7].Value = "QTY PER PACK";
            sheet.Cells[1, 8].Value = "";
            sheet.Cells[1, 9].Value = "OTHER NOTES";

            // Format Headers
            using (var range = sheet.Cells[1, 1, 1, 9])
            {
                range.Style.Font.Bold = true;
                range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.Font.Size = 12;
            }

            for (var c = 1; c <= 9; c++)
                sheet.Cells[1, c].Style.Border.BorderAround(ExcelBorderStyle.Medium);

            sheet.DefaultRowHeight = 18;
            col.BlankNotes = 8;
        }

        var nameFormula = sheet.Cells[2, col.Item_Name].Formula != "";
        var designFormula = sheet.Cells[2, col.Design].Formula != "";

        // Clear data from old sheet.
        if (!newSheet)
        {
            sheet.Cells[2, col.RackLocation, sheet.Dimension.End.Row, col.RackLocation].Clear();
            sheet.Cells[2, col.Sku, sheet.Dimension.End.Row, col.Sku].Clear();
            if (!nameFormula) sheet.Cells[2, col.Item_Name, sheet.Dimension.End.Row, col.Item_Name].Clear();
            if (!designFormula) sheet.Cells[2, col.Design, sheet.Dimension.End.Row, col.Design].Clear();
            sheet.Cells[2, col.QtyInLocation, sheet.Dimension.End.Row, col.QtyInLocation].Clear();
            sheet.Cells[2, col.QtyPerPack, sheet.Dimension.End.Row, col.QtyPerPack].Clear();
            sheet.Cells[2, col.BlankNotes, sheet.Dimension.End.Row, col.BlankNotes].Clear();
            sheet.Cells[2, col.OtherNotes, sheet.Dimension.End.Row, col.OtherNotes].Clear();
        }

        // Fill data
        var row = 1;
        foreach (var mixedCartonVM in MixedCartons)
        {
            var totalLines = mixedCartonVM.Bins.Sum(b => b.Stock.Count);
            var mid = (int)Math.Ceiling(totalLines / 2.0);
            var line = 0;
            var qtyNote = mixedCartonVM.Bins.Count > 1
                ? $"{mixedCartonVM.Cases} PACKS IN TOTAL"
                : "THIS IS ALL MIXED PACKS ON HAND";
            var startRow = row + 1;
            foreach (var mcBinVM in mixedCartonVM.Bins)
            {
                foreach (var mcStockVM in mcBinVM.Stock)
                {
                    line++;
                    row++;

                    sheet.Cells[row, col.RackLocation].Value = mcBinVM.Code;
                    sheet.Cells[row, col.Sku].Value = mcStockVM.Sku;
                    if (newSheet || !nameFormula) sheet.Cells[row, col.Item_Name].Value = mcStockVM.ItemName;
                    if (newSheet || !designFormula) sheet.Cells[row, col.Design].Value = mixedCartonVM.Name;
                    sheet.Cells[row, col.QtyInLocation].Value = mcStockVM.LocationQty;
                    sheet.Cells[row, col.ShippablePack].Value = "YES";
                    sheet.Cells[row, col.QtyPerPack].Value = mcStockVM.QtyPerCarton;
                    sheet.Cells[row, col.BlankNotes].Value = line == mid ? qtyNote : "";
                    sheet.Cells[row, col.OtherNotes].Value = mcStockVM.Note;
                    
                    for (var c = 1; c <= 9; c++)
                        sheet.Cells[row, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
            }

            // Format MC Area
            using var range = sheet.Cells[startRow, 1, row, 9];
            range.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Style.Font.Size = 11;
        }

        if (newSheet)
            for (var i = 1; i <= 9; i++)
                sheet.Column(i).AutoFit();

        package.SaveAs(xlFile);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}