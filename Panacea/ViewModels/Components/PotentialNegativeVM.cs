using Panacea.Models;
using Panacea.Properties;
using Panacea.ViewModels.Commands;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Panacea.Interfaces;
using Uranus;
using Uranus.Annotations;
using Uranus.Inventory;

namespace Panacea.ViewModels.Components;

public class PotentialNegativeVM : INotifyPropertyChanged, IChecker
{
    public Helios Helios { get; set; }
    public List<PotentNegCheckResult> ShortPickResults { get; set; }
    public List<PotentNegCheckResult> ReplenResults { get; set; }

    public PotentNegResultListVM ShortPickVM { get; set; }
    public PotentNegResultListVM ReplenishmentVM { get; set; }

    #region INotifyPropertChanged Members

    private string checkZoneString;
    public string CheckZoneString
    {
        get => checkZoneString;
        set
        {
            checkZoneString = value;
            OnPropertyChanged();
            Settings.Default.PotentNegZones = value;
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
            Settings.Default.PotentNegLocations = value;
            Settings.Default.Save();
        }
    }

    #endregion

    #region Commands

    public RunChecksCommand RunChecksCommand { get; set; }

    #endregion

    public PotentialNegativeVM(Helios helios)
    {
        Helios = helios;

        checkZoneString = Settings.Default.PotentNegZones;
        checkLocString = Settings.Default.PotentNegLocations;
        ShortPickResults = new List<PotentNegCheckResult>();
        ReplenResults = new List<PotentNegCheckResult>();

        ShortPickVM = new PotentNegResultListVM("Short Pick signs if not enough.");
        ReplenishmentVM = new PotentNegResultListVM("Organize Replenishment if not enough.");

        RunChecksCommand = new RunChecksCommand(this);
    }

    public void RunChecks()
    {
        Mouse.OverrideCursor = Cursors.Wait;

        ShortPickResults.Clear();
        ReplenResults.Clear();

        var zones = checkZoneString.ToUpper().Split(',', '|').ToList();
        var locations = checkLocString.ToUpper().Split(',', '|').ToList();

        // Pull dataSet.
        var dataSet = Helios.InventoryReader.BasicStockDataSet(zones, locations);
        if (dataSet is null)
        {
            MessageBox.Show("Failed to pull relevant data.");
            return;
        }

        foreach (var stock in dataSet.Stock.Where(s => s.Zone?.ZoneType == EZoneType.Pick && s.BasePickQty > 0 && s.BaseAvailableQty == 0))
        {
            var result = new PotentNegCheckResult(stock);
            if (stock.Item?.AvailableStock?.BaseQty > stock.BaseQty)
                ReplenResults.Add(result);
            else
                ShortPickResults.Add(result);
        }

        ShortPickVM.SetResults(ShortPickResults);
        ReplenishmentVM.SetResults(ReplenResults);

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}