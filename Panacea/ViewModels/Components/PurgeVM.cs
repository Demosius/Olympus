using Panacea.Interfaces;
using Panacea.Models;
using Panacea.Properties;
using Panacea.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Serilog;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Panacea.ViewModels.Components;

public class PurgeVM : INotifyPropertyChanged, IFilters, IBinData, IItemData, IChecker
{
    public Helios Helios { get; set; }
    public List<PurgeCheckResult> CheckResults { get; set; }

    #region INotifyPropertyChanged Members

    private string checkZoneString;
    public string CheckZoneString
    {
        get => checkZoneString;
        set
        {
            checkZoneString = value;
            OnPropertyChanged();
            Settings.Default.PurgeZones = value;
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
            Settings.Default.PurgeZones = value;
            Settings.Default.Save();
        }
    }

    private ObservableCollection<PurgeCheckResult> filteredCheckResults;
    public ObservableCollection<PurgeCheckResult> FilteredCheckResults
    {
        get => filteredCheckResults;
        set
        {
            filteredCheckResults = value;
            OnPropertyChanged();
        }
    }


    private string zoneTypeFilter;
    public string ZoneTypeFilter
    {
        get => zoneTypeFilter;
        set
        {
            zoneTypeFilter = value;
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

    #endregion

    #region Commands

    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public RunChecksCommand RunChecksCommand { get; set; }
    public BinsToClipboardCommand BinsToClipboardCommand { get; set; }
    public ItemsToClipboardCommand ItemsToClipboardCommand { get; set; }

    #endregion

    public PurgeVM(Helios helios)
    {
        Helios = helios;

        CheckResults = new List<PurgeCheckResult>();
        checkZoneString = Settings.Default.PurgeZones;
        checkLocString = Settings.Default.PurgeLocations;
        filteredCheckResults = new ObservableCollection<PurgeCheckResult>();
        zoneFilter = string.Empty;
        zoneTypeFilter = string.Empty;

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        RunChecksCommand = new RunChecksCommand(this);
        ItemsToClipboardCommand = new ItemsToClipboardCommand(this);
        BinsToClipboardCommand = new BinsToClipboardCommand(this);
    }

    public void ClearFilters()
    {
        ZoneFilter = string.Empty;
        ZoneTypeFilter = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        IEnumerable<PurgeCheckResult> results = CheckResults;

        if (ZoneFilter != string.Empty)
        {
            var regex = new Regex(ZoneFilter, RegexOptions.IgnoreCase);
            results = results.Where(res => regex.IsMatch(res.Zone));
        }

        if (ZoneTypeFilter != string.Empty)
        {
            var regex = new Regex(ZoneTypeFilter, RegexOptions.IgnoreCase);
            results = results.Where(res => regex.IsMatch(res.ZoneType.ToString()));
        }

        FilteredCheckResults = new ObservableCollection<PurgeCheckResult>(results);
    }

    public void ApplySorting()
    {
        throw new NotImplementedException();
    }

    public void RunChecks()
    {
        Mouse.OverrideCursor = Cursors.Wait;

        CheckResults.Clear();

        var zones = checkZoneString.ToUpper().Split(',', '|').ToList();
        var locations = checkLocString.ToUpper().Split(',', '|').ToList();

        // Pull dataSet.
        BasicStockDataSet? dataSet;
        try
        {
            dataSet = Helios.InventoryReader.BasicStockDataSet(zones, locations);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Missing Data");
            Log.Error(ex, "Error pulling data for purge.");
            Mouse.OverrideCursor = Cursors.Arrow;
            return;
        }
        if (dataSet is null)
        {
            MessageBox.Show("Failed to pull relevant data.");
            Mouse.OverrideCursor = Cursors.Arrow;
            return;
        }

        foreach (var (_, item) in dataSet.Items)
        {
            if ((item.Stock?.BaseQty ?? -1) != 0) continue;

            foreach (var (_, stock) in item.StockDict)
            {
                if (stock.NonCommitted && stock.BaseQty == 0)
                    CheckResults.Add(new PurgeCheckResult(stock));
            }
        }

        // Show results.
        ApplyFilters();

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public void BinsToClipboard()
    {
        var binList = FilteredCheckResults.Select(checkResult => checkResult.Bin).ToList();
        Clipboard.SetText(string.Join("|", binList));
        MessageBox.Show($"{binList.Count} bins added to clipboard.");
    }

    public void ItemsToClipboard()
    {
        var binList = FilteredCheckResults.Select(checkResult => checkResult.Item).ToList();
        Clipboard.SetText(string.Join("|", binList));
        MessageBox.Show($"{binList.Count} items added to clipboard.");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}