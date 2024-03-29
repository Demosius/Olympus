﻿using Hydra.Data;
using Hydra.Helpers;
using Hydra.Interfaces;
using Hydra.ViewModels.Commands;
using Microsoft.Win32;
using Morpheus;
using Morpheus.Helpers;
using Styx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Cadmus.Helpers;
using Cadmus.Interfaces;
using Cadmus.Models;
using Cadmus.ViewModels.Commands;
using Cadmus.ViewModels.Labels;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels.Controls;

public class RunVM : INotifyPropertyChanged, IDBInteraction, IItemFilters, IExport
{
    public HydraVM HydraVM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    private HydraDataSet? dataSet;

    public ObservableCollection<SiteVM> Sites { get; set; }

    public List<MoveVM> AllMoves { get; set; }

    // Track whether the current data is old data that has been loaded which likely
    // includes different core data (bin contents) than the current database holds.
    public bool OldLoaded { get; set; }

    public bool CanExport => CurrentMoves.Any();

    #region InotifyPropertyChanged Members

    private ObservableCollection<MoveVM> currentMoves;
    public ObservableCollection<MoveVM> CurrentMoves
    {
        get => currentMoves;
        set
        {
            currentMoves = value;
            OnPropertyChanged();
        }
    }

    private string itemFilterString;
    public string ItemFilterString
    {
        get => itemFilterString;
        set
        {
            itemFilterString = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }


    private string fromSiteFilterString;
    public string FromSiteFilterString
    {
        get => fromSiteFilterString;
        set
        {
            fromSiteFilterString = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }


    private string toSiteFilterString;
    public string ToSiteFilterString
    {
        get => toSiteFilterString;
        set
        {
            toSiteFilterString = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public FilterItemsFromClipboardCommand FilterItemsFromClipboardCommand { get; set; }
    public GenerateMovesCommand GenerateMovesCommand { get; set; }
    public ExportToCSVCommand ExportToCSVCommand { get; set; }
    public ExportToPDFCommand ExportToPDFCommand { get; set; }
    public ExportToExcelCommand ExportToExcelCommand { get; set; }
    public ExportToLabelsCommand ExportToLabelsCommand { get; set; }
    public SaveGenerationCommand SaveGenerationCommand { get; set; }

    #endregion

    private RunVM(HydraVM hydraVM, Helios helios, Charon charon)
    {
        HydraVM = hydraVM;

        Helios = helios;
        Charon = charon;

        Sites = new ObservableCollection<SiteVM>();
        AllMoves = new List<MoveVM>();
        currentMoves = new ObservableCollection<MoveVM>();

        itemFilterString = string.Empty;
        fromSiteFilterString = string.Empty;
        toSiteFilterString = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        FilterItemsFromClipboardCommand = new FilterItemsFromClipboardCommand(this);
        GenerateMovesCommand = new GenerateMovesCommand(this);
        ExportToCSVCommand = new ExportToCSVCommand(this);
        ExportToPDFCommand = new ExportToPDFCommand(this);
        ExportToExcelCommand = new ExportToExcelCommand(this);
        ExportToLabelsCommand = new ExportToLabelsCommand(this);
        SaveGenerationCommand = new SaveGenerationCommand(this);
    }

    public RunVM(HydraVM hydraVM, Helios helios, Charon charon, IEnumerable<Site> sites) : this(hydraVM, helios, charon)
    {
        Sites = new ObservableCollection<SiteVM>(sites.Select(s => new SiteVM(s)));
    }

    private async Task<RunVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<RunVM> CreateAsync(HydraVM hydraVM, Helios helios, Charon charon)
    {
        var ret = new RunVM(hydraVM, helios, charon);
        return ret.InitializeAsync();
    }

    public async Task RefreshDataAsync()
    {
        Sites.Clear();
        var (sites, _) = await Helios.InventoryReader.SitesAsync();
        foreach (var site in sites)
            Sites.Add(new SiteVM(site));
    }

    public void ClearFilters()
    {
        itemFilterString = string.Empty;
        fromSiteFilterString = string.Empty;
        toSiteFilterString = string.Empty;
        CurrentMoves = new ObservableCollection<MoveVM>(AllMoves);
        OnPropertyChanged(nameof(ItemFilterString));
        OnPropertyChanged(nameof(FromSiteFilterString));
        OnPropertyChanged(nameof(ToSiteFilterString));
    }

    public void ApplyFilters()
    {
        var itemRegex = new Regex(ItemFilterString);
        var fromRegex = new Regex(FromSiteFilterString);
        var toRegex = new Regex(ToSiteFilterString);

        CurrentMoves = new ObservableCollection<MoveVM>(AllMoves.Where(m =>
            itemRegex.IsMatch(m.ItemNumber.ToString()) &&
            fromRegex.IsMatch(m.TakeSiteName) &&
            toRegex.IsMatch(m.PlaceSiteName)));
    }

    public void FilterItemsFromClipboard()
    {
        Mouse.OverrideCursor = Cursors.Wait;
        var numbers = new List<int>();

        // Set data.
        var rawData = General.ClipboardToString();
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        var stream = new MemoryStream(byteArray);
        using var reader = new StreamReader(stream);

        // Get the item number column, if there is one.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();

        var itemIndex = Array.IndexOf(headArr, "Item No.");
        if (itemIndex == -1) itemIndex = Array.IndexOf(headArr, "Item Number");
        if (itemIndex == -1) itemIndex = Array.IndexOf(headArr, "Item");

        if (itemIndex == -1)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            MessageBox.Show("Could not detect item number values within clipboard data.");
            return;
        }

        line = reader.ReadLine();

        while (line is not null)
        {
            var row = line.Split('\t');

            if (int.TryParse(row[itemIndex], out var itemNumber)) numbers.Add(itemNumber);

            line = reader.ReadLine();
        }

        numbers.Sort();

        const int x = 3000;

        MessageBox.Show(numbers.Count <= x
            ? $"Found {numbers.Count:#,###} potential item numbers."
            : $"Found {numbers.Count:#,###} potential item numbers. Will only use the first {x:#,###}.");

        ItemFilterString = string.Join("|", numbers.Select(n => n.ToString("000000")).Take(x));
        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public async Task GenerateMoves()
    {
        Mouse.OverrideCursor = Cursors.Wait;
        dataSet = await Helios.InventoryReader.HydraDataSetAsync();

        if (dataSet is null) return;

        var siteMoves =
            MoveGenerator.GenerateSiteMoves(dataSet, Sites.Where(s => s.TakeFrom).Select(s => s.Name),
                Sites.Where(s => s.PlaceTo).Select(s => s.Name));

        AllMoves = siteMoves.Select(m => new MoveVM(m)).ToList();
        ApplyFilters();
        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public async Task SaveGeneration()
    {
        var dir = Path.Combine(Helios.SolLocation, "Inventory", "Hydra");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        var dialog = new SaveFileDialog
        {
            Filter = "SQLite Files (*.sqlite)|*.sqlite",
            Title = "Save Generated Moves",
            InitialDirectory = dir,
            FileName = $"{DefaultExportString}.sqlite"
        };

        if (dialog.ShowDialog() != true) return;

        var filePath = dialog.FileName;

        var chariot = await Task.Run(() => new HydraChariot(filePath));

        await chariot.SendDataAsync(dataSet, AllMoves.Select(vm => vm.Move));
    }

    public async Task ExportToLabels()
    {
        await Task.Run(() =>
        {
            // Convert to LabelVM
            var labels = new List<ReceivingPutAwayLabelVM>();

            foreach (var moveVM in CurrentMoves)
            {
                var move = moveVM.Move;
                var labelCount = new List<int> { move.TakeCases + move.TakePacks + move.TakeEaches, 4 }.AsQueryable().Min();

                for (var i = 0; i < labelCount; i++)
                {
                    var label = new ReceivingPutAwayLabel(move) { LabelTotal = labelCount, LabelNumber = i + 1 };
                    var labelVM = new ReceivingPutAwayLabelVM(label);
                    labels.Add(labelVM);
                }
            }

            PrintUtility.PrintLabels(labels, null);
        });
    }

    public async Task ExportToExcel()
    {
        await Task.Run(() => Output.DataTableToExcel(CurrentMoveDataTable(), DefaultExportString));
    }

    public async Task ExportToPDF()
    {
        await Task.Run(() =>
        {
            Output.MovesToPDF(CurrentMoves
                    .Select(vm => vm.Move)
                    .GroupBy(m => m.TakeSite?.Name ?? "")
                    .ToDictionary(g => g.Key, g => g.OrderBy(move => move.TakeBin?.Code ?? "").ToList()),
                DefaultExportString);
        });
    }

    public async Task ExportToCSV()
    {
        await Task.Run(() => Output.DataTableToCSV(CurrentMoveDataTable(), DefaultExportString));
    }

    private static string DefaultExportString => $"hydra_export_{DateTime.Now:yyyyMMddTHHmmss}";

    private DataTable CurrentMoveDataTable()
    {
        var dt = new DataTable();

        dt.Columns.Add(new DataColumn("Item Number"));
        dt.Columns.Add(new DataColumn("Item Description"));
        dt.Columns.Add(new DataColumn("From Site"));
        dt.Columns.Add(new DataColumn("From Location"));
        dt.Columns.Add(new DataColumn("From Zone"));
        dt.Columns.Add(new DataColumn("From Bay"));
        dt.Columns.Add(new DataColumn("From Bin"));
        dt.Columns.Add(new DataColumn("To Site"));
        dt.Columns.Add(new DataColumn("To Location"));
        dt.Columns.Add(new DataColumn("To Zone"));
        dt.Columns.Add(new DataColumn("To Bay"));
        dt.Columns.Add(new DataColumn("To Bin"));
        dt.Columns.Add(new DataColumn("Take Cases"));
        dt.Columns.Add(new DataColumn("Take Packs"));
        dt.Columns.Add(new DataColumn("Take Eaches"));
        dt.Columns.Add(new DataColumn("Place Cases"));
        dt.Columns.Add(new DataColumn("Place Packs"));
        dt.Columns.Add(new DataColumn("Place Eaches"));

        foreach (var move in CurrentMoves.Select(vm => vm.Move))
        {
            var row = dt.NewRow();
            row["Item Number"] = move.ItemNumber;
            row["Item Description"] = move.Item?.Description;
            row["From Site"] = move.TakeSite?.Name;
            row["From Location"] = move.TakeLocation;
            row["From Zone"] = move.TakeZone?.Code;
            row["From Bay"] = move.TakeBay?.ID;
            row["From Bin"] = move.TakeBin?.Code;
            row["To Site"] = move.PlaceSite?.Name;
            row["To Location"] = move.PlaceLocation;
            row["To Zone"] = move.PlaceZone?.Code;
            row["To Bay"] = move.PlaceBay?.ID;
            row["To Bin"] = move.PlaceBin?.Code;
            row["Take Cases"] = move.TakeCases;
            row["Take Packs"] = move.TakePacks;
            row["Take Eaches"] = move.TakeEaches;
            row["Place Cases"] = move.PlaceCases;
            row["Place Packs"] = move.PlacePacks;
            row["Place Eaches"] = move.PlaceEaches;

            dt.Rows.Add(row);
        }

        return dt;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}