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

namespace Panacea.ViewModels.Components;

public class BinsWithMultipleItemsVM : INotifyPropertyChanged, IFilters, IBinData, IChecker
{
    public Helios Helios { get; set; }
    public List<BWMICheckResult> CheckResults { get; set; }

    #region INotifyPropertyChanged Members

    private string checkZoneString;
    public string CheckZoneString
    {
        get => checkZoneString;
        set
        {
            checkZoneString = value;
            OnPropertyChanged();
            Settings.Default.BWMIZones = value;
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
            Settings.Default.BWMILocations = value;
            Settings.Default.Save();
        }
    }

    private ObservableCollection<BWMICheckResult> filteredCheckResults;
    public ObservableCollection<BWMICheckResult> FilteredCheckResults
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
    public BinsToClipboardCommand BinsToClipboardCommand { get; set; }
    public RunChecksCommand RunChecksCommand { get; set; }

    #endregion

    public BinsWithMultipleItemsVM(Helios helios)
    {
        Helios = helios;

        CheckResults = new List<BWMICheckResult>();
        checkZoneString = Settings.Default.BWMIZones;
        checkLocString = Settings.Default.BWMILocations;
        filteredCheckResults = new ObservableCollection<BWMICheckResult>();

        zoneFilter = string.Empty;
        zoneTypeFilter = string.Empty;

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        BinsToClipboardCommand = new BinsToClipboardCommand(this);
        RunChecksCommand = new RunChecksCommand(this);
    }

    public void ClearFilters()
    {
        ZoneFilter = string.Empty;
        ZoneTypeFilter = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        IEnumerable<BWMICheckResult> results = CheckResults;

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

        FilteredCheckResults = new ObservableCollection<BWMICheckResult>(results);
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
        var dataSet = Helios.InventoryReader.BasicStockDataSet(zones, locations);
        if (dataSet is null)
        {
            MessageBox.Show("Failed to pull relevant data.");
            return;
        }

        foreach (var (_, bin) in dataSet.Bins)
        {
            if (bin.Stock.Count > 1)
            {
                CheckResults.Add(new BWMICheckResult(bin));
            }
        }

        // Show results.
        ApplyFilters();

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public void BinsToClipboard()
    {
        var binList = FilteredCheckResults.Select(checkResult => checkResult.Bin.Code).ToList();
        Clipboard.SetText(string.Join("|", binList));
        MessageBox.Show($"{binList.Count} bins added to clipboard.");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}