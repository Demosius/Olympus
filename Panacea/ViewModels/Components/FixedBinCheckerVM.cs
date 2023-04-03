using Panacea.Models;
using Panacea.Properties;
using Panacea.ViewModels.Commands;
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
using Uranus.Inventory.Models;

namespace Panacea.ViewModels.Components;

public class FixedBinCheckerVM : INotifyPropertyChanged, IFilters
{
    public Helios Helios { get; set; }
    public List<FixedBinCheckResult> CheckResults { get; set; }

    #region INotifyPropertyChanged Members

    private string fromZoneString;
    public string FromZoneString
    {
        get => fromZoneString;
        set
        {
            fromZoneString = value;
            OnPropertyChanged();
            Settings.Default.FBCFromZones = value;
            Settings.Default.Save();
        }
    }

    private string fixedZoneString;
    public string FixedZoneString
    {
        get => fixedZoneString;
        set
        {
            fixedZoneString = value;
            OnPropertyChanged();
            Settings.Default.FBCFixedZones = value;
            Settings.Default.Save();
        }
    }


    private bool checkCase;
    public bool CheckCase
    {
        get => checkCase;
        set
        {
            checkCase = value;
            OnPropertyChanged();
        }
    }


    private bool checkPack;
    public bool CheckPack
    {
        get => checkPack;
        set
        {
            checkPack = value;
            OnPropertyChanged();
        }
    }


    private bool checkEach;
    public bool CheckEach
    {
        get => checkEach;
        set
        {
            checkEach = value;
            OnPropertyChanged();
            if (checkExclusiveEach && checkEach) CheckExclusiveEach = false;
        }
    }

    private bool checkExclusiveEach;
    public bool CheckExclusiveEach
    {
        get => checkExclusiveEach;
        set
        {
            checkExclusiveEach = value;
            OnPropertyChanged();
            if (checkExclusiveEach && checkEach) CheckEach = false;
        }
    }

    private ObservableCollection<FixedBinCheckResult> filteredCheckResults;
    public ObservableCollection<FixedBinCheckResult> FilteredCheckResults
    {
        get => filteredCheckResults;
        set
        {
            filteredCheckResults = value;
            OnPropertyChanged();
        }
    }

    private bool? passFilter;
    public bool? PassFilter
    {
        get => passFilter;
        set
        {
            passFilter = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }


    private string binFilter;
    public string BinFilter
    {
        get => binFilter;
        set
        {
            binFilter = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    #endregion

    #region Commands

    public RunFixedBinChecksCommand RunFixedBinChecksCommand { get; set; }

    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }

    #endregion

    public FixedBinCheckerVM(Helios helios)
    {
        Helios = helios;
        fromZoneString = Settings.Default.FBCFromZones;
        fixedZoneString = Settings.Default.FBCFixedZones;
        CheckResults = new List<FixedBinCheckResult>();
        filteredCheckResults = new ObservableCollection<FixedBinCheckResult>();
        binFilter = string.Empty;

        RunFixedBinChecksCommand = new RunFixedBinChecksCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    public void RunChecks()
    {
        Mouse.OverrideCursor = Cursors.Wait;

        CheckResults.Clear();

        var fromZones = fromZoneString.ToUpper().Split(',', '|').ToList();
        var fixedZones = fixedZoneString.ToUpper().Split(',', '|').ToList();

        // Pull dataSet.
        var dataSet = Helios.InventoryReader.FixedBinCheckDataSet(fromZones, fixedZones);
        if (dataSet is null)
        {
            MessageBox.Show("Failed to pull relevant data.");
            return;
        }

        // Get items only that exist in from zones.
        var items = new List<NAVItem>();
        foreach (var (_, item) in dataSet.Items)
        {
            if (item.StockDict.Values.Any(stock => fromZones.Contains(stock.Bin?.ZoneCode ?? "")))
                items.Add(item);
        }

        // Convert items in dataSet to result collection.
        foreach (var item in items) CheckResults.Add(new FixedBinCheckResult(item, fixedZones));

        // Run checks against results.
        foreach (var result in CheckResults)
        {
            result.RunChecks(checkCase, checkPack, checkEach, checkExclusiveEach);
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

    public void ClearFilters()
    {
        PassFilter = null;
        BinFilter = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        IEnumerable<FixedBinCheckResult> results = CheckResults;

        if (PassFilter is not null)
            results = results.Where(res => PassFilter == res.PassCheck);

        if (BinFilter != string.Empty)
        {
            var regex = new Regex(BinFilter, RegexOptions.IgnoreCase);
            results = results.Where(res => regex.IsMatch(res.FixedBins));
        }

        FilteredCheckResults = new ObservableCollection<FixedBinCheckResult>(results);
    }
}