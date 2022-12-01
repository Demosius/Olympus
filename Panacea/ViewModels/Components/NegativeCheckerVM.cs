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

public class NegativeCheckerVM : INotifyPropertyChanged, IFilters, IBinData, IItemData
{
    public Helios Helios { get; set; }
    public List<NegativeCheckResult> CheckResults { get; set; }

    #region INotifyPropertyChanged Members

    private string checkZoneString;
    public string CheckZoneString
    {
        get => checkZoneString;
        set
        {
            checkZoneString = value;
            OnPropertyChanged();
            Settings.Default.NegativeZones = value;
            Settings.Default.Save();
        }
    }

    private ObservableCollection<NegativeCheckResult> filteredCheckResults;
    public ObservableCollection<NegativeCheckResult> FilteredCheckResults
    {
        get => filteredCheckResults;
        set
        {
            filteredCheckResults = value;
            OnPropertyChanged();
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


    private bool trueNegative;
    public bool TrueNegative
    {
        get => trueNegative;
        set
        {
            trueNegative = value;
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
    public ItemsToClipboardCommand ItemsToClipboardCommand { get; set; }
    public RunNegativeChecksCommand RunNegativeChecksCommand { get; set; }

    #endregion

    public NegativeCheckerVM(Helios helios)
    {
        Helios = helios;

        CheckResults = new List<NegativeCheckResult>();
        checkZoneString = Settings.Default.NegativeZones;
        filteredCheckResults = new ObservableCollection<NegativeCheckResult>();
        zoneFilter = string.Empty;

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        BinsToClipboardCommand = new BinsToClipboardCommand(this);
        ItemsToClipboardCommand = new ItemsToClipboardCommand(this);
        RunNegativeChecksCommand = new RunNegativeChecksCommand(this);
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


    public void ClearFilters()
    {
        ZoneFilter = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        IEnumerable<NegativeCheckResult> results = CheckResults;

        if (ZoneFilter != string.Empty)
        {
            var regex = new Regex(ZoneFilter, RegexOptions.IgnoreCase);
            results = results.Where(res => regex.IsMatch(res.Zone));
        }

        if (TrueNegative) results = results.Where(res => res.BalanceQty < 0);

        FilteredCheckResults = new ObservableCollection<NegativeCheckResult>(results);
    }

    public void ApplySorting()
    {
        throw new NotImplementedException();
    }

    public void RunNegativeChecks()
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

        foreach (var stock in dataSet.Stock.Where(stock => stock.Zone?.ZoneType == EZoneType.Pick && stock.HasNegativeUoM))
        {
            if (stock.Eaches?.IsNegative ?? false) CheckResults.Add(new NegativeCheckResult(stock, EUoM.EACH));
            if (stock.Packs?.IsNegative ?? false) CheckResults.Add(new NegativeCheckResult(stock, EUoM.PACK));
            if (stock.Cases?.IsNegative ?? false) CheckResults.Add(new NegativeCheckResult(stock, EUoM.CASE));
        }

        // Show results.
        ApplyFilters();

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}