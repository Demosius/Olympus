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
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory;

namespace Panacea.ViewModels.Components;

public class ItemsWithMultipleBinsVM : INotifyPropertyChanged, IFilters, IItemData
{
    public Helios Helios { get; set; }
    public List<IWMBCheckResult> CheckResults { get; set; }

    #region INotifyPropertyChanged Members

    private string checkZoneString;
    public string CheckZoneString
    {
        get => checkZoneString;
        set
        {
            checkZoneString = value;
            OnPropertyChanged();
            Settings.Default.IWMBZones = value;
            Settings.Default.Save();
        }
    }

    private bool allowSeparatedUoMs;
    public bool AllowSeparatedUoMs
    {
        get => allowSeparatedUoMs;
        set
        {
            allowSeparatedUoMs = value;
            OnPropertyChanged();
            Settings.Default.IWMBAllowSeparateUoMs = value;
            Settings.Default.Save();
        }
    }


    private ObservableCollection<IWMBCheckResult> filteredCheckResults;
    public ObservableCollection<IWMBCheckResult> FilteredCheckResults
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
    public RunIWMBChecksCommand RunIWMBChecksCommand { get; set; }
    public ItemsToClipboardCommand ItemsToClipboardCommand { get; set; }

    #endregion


    public ItemsWithMultipleBinsVM(Helios helios)
    {
        Helios = helios;

        checkZoneString = Settings.Default.IWMBZones;
        allowSeparatedUoMs = Settings.Default.IWMBAllowSeparateUoMs;

        CheckResults = new List<IWMBCheckResult>();
        filteredCheckResults = new ObservableCollection<IWMBCheckResult>();

        zoneFilter = string.Empty;
        zoneTypeFilter = string.Empty;

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        RunIWMBChecksCommand = new RunIWMBChecksCommand(this);
        ItemsToClipboardCommand = new ItemsToClipboardCommand(this);
    }

    public void RunIWMBChecks()
    {
        Mouse.OverrideCursor = Cursors.Wait;

        CheckResults.Clear();

        var zones = checkZoneString.ToUpper().Split(',', '|').ToList();

        // Pull dataSet.
        var dataSet = Helios.InventoryReader.BasicStockDataSet(zones);
        if (dataSet is null)
        {
            MessageBox.Show("Failed to pull relevant data.");
            return;
        }

        foreach (var (_, item) in dataSet.Items)
        {
            foreach (EZoneType zoneType in Enum.GetValues(typeof(EZoneType)))
            {
                var result = new IWMBCheckResult(item, zoneType, AllowSeparatedUoMs);
                if (result.HasMultipleBins) CheckResults.Add(result);
            }
        }

        // Show results.
        ApplyFilters();

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public void ClearFilters()
    {
        ZoneFilter = string.Empty;
        ZoneTypeFilter = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        IEnumerable<IWMBCheckResult> results = CheckResults;

        if (ZoneFilter != string.Empty)
        {
            var regex = new Regex(ZoneFilter, RegexOptions.IgnoreCase);
            results = results.Where(res => regex.IsMatch(res.ZoneString));
        }

        if (ZoneTypeFilter != string.Empty)
        {
            var regex = new Regex(ZoneTypeFilter, RegexOptions.IgnoreCase);
            results = results.Where(res => regex.IsMatch(res.ZoneType.ToString()));
        }

        FilteredCheckResults = new ObservableCollection<IWMBCheckResult>(results);
    }

    public void ApplySorting()
    {
        throw new NotImplementedException();
    }

    public void ItemsToClipboard()
    {
        var itemList = FilteredCheckResults.Select(checkResult => checkResult.Item.Number.ToString()).ToList();
        Clipboard.SetText(string.Join("|", itemList));
        MessageBox.Show($"{itemList.Count} items added to clipboard.");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}