using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Morpheus;
using Panacea.Interfaces;
using Panacea.Models;
using Panacea.Properties;
using Panacea.ViewModels.Commands;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Panacea.ViewModels.Components;

public class ItemsWithNoPickBinVM : INotifyPropertyChanged, IFilters, IItemData, IChecker
{
    public Helios Helios { get; set; }

    public List<IWNPBCheckResult> CheckResults { get; set; }

    public TOStockDataSet? DataSet { get; set; }

    #region INotifyPropertyChanged Members

    private string checkZoneString;
    public string CheckZoneString
    {
        get => checkZoneString;
        set
        {
            checkZoneString = value;
            OnPropertyChanged();
            Settings.Default.PickBinCheckZones = value;
            Settings.Default.Save();
        }
    }

    private string checkLocString;
    public string CheckLocString
    {
        get => checkLocString;
        set
        {
            checkLocString = value;
            OnPropertyChanged();
            Settings.Default.PickBinCheckLocations = value;
            Settings.Default.Save();
        }
    }

    private ObservableCollection<IWNPBCheckResult> filteredCheckResults;
    public ObservableCollection<IWNPBCheckResult> FilteredCheckResults
    {
        get => filteredCheckResults;
        set
        {
            filteredCheckResults = value;
            OnPropertyChanged();
        }
    }

    private string itemFilter;
    public string ItemFilter
    {
        get => itemFilter;
        set
        {
            itemFilter = value;
            OnPropertyChanged();
            ApplyFilters();
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
            ApplyFilters();
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
            ApplyFilters();
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
            ApplyFilters();
        }
    }

    private string genreFilter;
    public string GenreFilter
    {
        get => genreFilter;
        set
        {
            genreFilter = value;
            OnPropertyChanged();
            ApplyFilters();
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
            ApplyFilters();
        }
    }

    private bool checkTOLines;
    public bool CheckTOLines
    {
        get => checkTOLines;
        set
        {
            checkTOLines = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public ItemsToClipboardCommand ItemsToClipboardCommand { get; set; }
    public RunChecksCommand RunChecksCommand { get; set; }
    public UpdateTOLinesCommand UpdateTOLinesCommand { get; set; }

    #endregion

    public ItemsWithNoPickBinVM(Helios helios)
    {
        Helios = helios;
        CheckResults = new List<IWNPBCheckResult>();
        filteredCheckResults = new ObservableCollection<IWNPBCheckResult>();
        checkZoneString = Settings.Default.PickBinCheckZones;
        checkLocString = Settings.Default.PickBinCheckLocations;
        zoneFilter = string.Empty;
        itemFilter = string.Empty;
        platformFilter = string.Empty;
        categoryFilter = string.Empty;
        genreFilter = string.Empty;
        divisionFilter = string.Empty;

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        ItemsToClipboardCommand = new ItemsToClipboardCommand(this);
        RunChecksCommand = new RunChecksCommand(this);
        UpdateTOLinesCommand = new UpdateTOLinesCommand(this);
    }

    public void ClearFilters()
    {
        ZoneFilter = string.Empty;
        ItemFilter = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        IEnumerable<IWNPBCheckResult> results = CheckResults;

        if (ZoneFilter != string.Empty)
        {
            var regex = new Regex(ZoneFilter, RegexOptions.IgnoreCase);
            results = results.Where(res => regex.IsMatch(res.ZoneString));
        }

        if (ItemFilter != string.Empty)
        {
            var regex = new Regex(ItemFilter, RegexOptions.IgnoreCase);
            results = results.Where(res => regex.IsMatch(res.ItemNumber.ToString()));
        }

        if (PlatformFilter != string.Empty)
        {
            var regex = new Regex(PlatformFilter, RegexOptions.IgnoreCase);
            results = results.Where(res => regex.IsMatch(res.Platform));
        }

        if (CategoryFilter != string.Empty)
        {
            var regex = new Regex(CategoryFilter, RegexOptions.IgnoreCase);
            results = results.Where(res => regex.IsMatch(res.Category));
        }


        if (DivisionFilter != string.Empty)
        {
            var regex = new Regex(DivisionFilter, RegexOptions.IgnoreCase);
            results = results.Where(res => regex.IsMatch(res.Division));
        }

        if (GenreFilter != string.Empty)
        {
            var regex = new Regex(GenreFilter, RegexOptions.IgnoreCase);
            results = results.Where(res => regex.IsMatch(res.Genre));
        }



        FilteredCheckResults = new ObservableCollection<IWNPBCheckResult>(results);
    }

    public void ApplySorting()
    {
        throw new NotImplementedException();
    }

    public void ItemsToClipboard()
    {
        var itemList = FilteredCheckResults.Select(checkResult => checkResult.ItemNumber).ToList();
        Clipboard.SetText(string.Join("|", itemList));
        MessageBox.Show($"{itemList.Count} items added to clipboard.");
    }

    public void RunChecks()
    {
        Mouse.OverrideCursor = Cursors.Wait;

        CheckResults.Clear();

        var zones = checkZoneString.ToUpper().Split(',', '|').ToList();
        var locations = checkLocString.ToUpper().Split(',', '|').ToList();

        // Pull dataSet.
        var dataSet = Helios.InventoryReader.TOStockDataSet(zones, locations);
        if (dataSet is null)
        {
            MessageBox.Show("Failed to pull relevant data.");
            return;
        }

        foreach (var (_, item) in dataSet.Items)
        {
            if (item.HasPickBin || (CheckTOLines && item.TODemandBaseQty == 0) || (item.Stock?.BaseQty ?? 0) <= 0) continue;
            CheckResults.Add(new IWNPBCheckResult(item));
        }

        // Show results.
        ApplyFilters();

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public void UpdateTOLines()
    {
        int updateLines;
        var raw = General.ClipboardToString();
        List<NAVTransferOrder> newTOLines;
        try
        {
            newTOLines = DataConversion.NAVRawStringToTransferOrders(raw);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (!newTOLines.Any())
        {
            MessageBox.Show("No TO Line data found on Clipboard.", "No Data", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }
        var count = Helios.InventoryReader.TOLineCount();
        if (count == 0)
        {
            updateLines = Helios.InventoryCreator.NAVTransferOrders(newTOLines);
        }
        else
        {
            var result = MessageBox.Show(
                $"{newTOLines.Count} TO lines discovered. Would you like to replace all previous TO data?\n\n(No: Data will be added alongside existing lines - which may no longer be relevant.)",
                "Replace Data?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    updateLines = Helios.InventoryCreator.NAVTransferOrders(newTOLines);
                    break;
                case MessageBoxResult.No:
                    updateLines = Helios.InventoryUpdater.NAVTransferOrders(newTOLines);
                    break;
                case MessageBoxResult.None:
                case MessageBoxResult.OK:
                case MessageBoxResult.Cancel:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        MessageBox.Show($"Updated {updateLines} lines.", "Update", MessageBoxButton.OK);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}