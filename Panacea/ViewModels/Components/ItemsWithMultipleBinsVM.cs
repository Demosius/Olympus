using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Panacea.Models;
using Panacea.Properties;
using Panacea.ViewModels.Commands;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Panacea.ViewModels.Components;

public class ItemsWithMultipleBinsVM : INotifyPropertyChanged, IFilters
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

    #endregion

    #region Commands

    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public RunIWMBChecksCommand RunIWMBChecksCommand { get; set; }

    #endregion


    public ItemsWithMultipleBinsVM(Helios helios)
    {
        Helios = helios;

        checkZoneString = Settings.Default.IWMBZones;
        allowSeparatedUoMs = Settings.Default.IWMBAllowSeparateUoMs;

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        RunIWMBChecksCommand = new RunIWMBChecksCommand(this);
    }

    public void RunIWMBChecks()
    {
        Mouse.OverrideCursor = Cursors.Wait;

        CheckResults.Clear();

        var zones = checkZoneString.ToUpper().Split(',', '|').ToList();

        // Pull dataSet.
        var dataSet = Helios.InventoryReader.IWMBDataSet(zones);
        if (dataSet is null)
        {
            MessageBox.Show("Failed to pull relevant data.");
            return;
        }

        /*// Get items only that exist in from zones.
        var items = new List<NAVItem>();
        foreach (var (_, item) in dataSet.Items)
        {
            if (item.StockDict.Values.Any(stock => fromZones.Contains(stock.Bin?.ZoneCode ?? "")))
                items.Add(item);
        }

        // Convert items in dataSet to result collection.
        foreach (var item in items) CheckResults.Add(new ItemCheckResult(item, fixedZones));

        // Run checks against results.
        foreach (var result in CheckResults)
        {
            result.RunChecks(checkCase, checkPack, checkEach, checkExclusiveEach);
        }*/

        // Show results.
        ApplyFilters();

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public void ClearFilters()
    {
        throw new NotImplementedException();
    }

    public void ApplyFilters()
    {
        throw new NotImplementedException();
    }

    public void ApplySorting()
    {
        throw new NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}