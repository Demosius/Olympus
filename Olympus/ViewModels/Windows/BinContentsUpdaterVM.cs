using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Cadmus.Annotations;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Controls;
using Morpheus.ViewModels.Interfaces;
using Uranus;
using Uranus.Commands;
using Uranus.Inventory.Models;

namespace Olympus.ViewModels.Windows;

public class BinContentsUpdaterVM : INotifyPropertyChanged, IMultiSelect, IConfirm
{
    public Helios Helios { get; set; }

    public List<NAVStock> NewStock { get; set; }

    public List<NAVStock> OldStock { get; set; }

    public List<string> MissingZonesList { get; set; }

    public bool ZonesMissing => MissingZonesList.Any();

    private readonly List<StringSelectorVM> allZones;

    public int SuccessfulUploadLines { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<StringSelectorVM> Zones { get; set; }
    
    private string filterString;
    public string FilterString
    {
        get => filterString;
        set
        {
            filterString = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    #endregion

    #region Commands

    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public SelectAllCommand SelectAllCommand { get; set; }
    public DeselectAllCommand DeselectAllCommand { get; set; }
    public SelectFilteredCommand SelectFilteredCommand { get; set; }
    public DeselectFilteredCommand DeselectFilteredCommand { get; set; }
    public SelectFilteredExclusiveCommand SelectFilteredExclusiveCommand { get; set; }
    public ConfirmCommand ConfirmCommand { get; set; }
    public ConfirmAndCloseCommand ConfirmAndCloseCommand { get; set; }

    #endregion

    public BinContentsUpdaterVM(Helios helios, List<NAVStock> newStock)
    {
        Helios = helios;
        NewStock = newStock;

        OldStock = AsyncHelper.RunSync(() => Helios.InventoryReader.NAVAllStockAsync());

        // What Zones are present in old data and not in new?
        var newZones = NewStock.Select(s => s.ZoneID).Distinct().ToList();
        var oldZones = OldStock.Select(s => s.ZoneID).Distinct().ToList();

        MissingZonesList =  oldZones.Where(zone => !newZones.Contains(zone)).ToList();

        allZones = MissingZonesList.Select(zone => new StringSelectorVM(zone)).ToList();
        Zones = new ObservableCollection<StringSelectorVM>();
        
        filterString = string.Empty;

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        SelectAllCommand = new SelectAllCommand(this);
        DeselectAllCommand = new DeselectAllCommand(this);
        SelectFilteredCommand = new SelectFilteredCommand(this);
        DeselectFilteredCommand = new DeselectFilteredCommand(this);
        SelectFilteredExclusiveCommand = new SelectFilteredExclusiveCommand(this);
        ConfirmCommand = new ConfirmCommand(this);
        ConfirmAndCloseCommand = new ConfirmAndCloseCommand(this);

        ApplyFilters();
    }

    public void ClearFilters()
    {
        FilterString = string.Empty;
    }

    public void ApplyFilters()
    {
        Zones.Clear();
        var zones = allZones.Where(z => Regex.IsMatch(z.Name, FilterString));

        foreach (var zone in zones) Zones.Add(zone);
    }

    public void SelectAll()
    {
        foreach (var zone in allZones)
            zone.Selected = true;
    }

    public void DeselectAll()
    {
        foreach (var zone in allZones)
            zone.Selected = false;
    }

    public void SelectFiltered()
    {
        foreach (var zone in Zones)
            zone.Selected = true;
    }

    public void DeselectFiltered()
    {
        foreach (var zone in Zones)
            zone.Selected = false;
    }

    public void SelectFilteredExclusive()
    {
        DeselectAll();
        SelectFiltered();
    }

    public bool Confirm()
    {
        Mouse.OverrideCursor = Cursors.Wait;
        SuccessfulUploadLines = AsyncHelper.RunSync(UploadStock);
        Mouse.OverrideCursor = Cursors.Arrow;

        return SuccessfulUploadLines > 0;
    }

    private async Task<int> UploadStock() =>
        await Helios.InventoryUpdater.NAVStockAsync(NewStock,
            allZones.Where(z => z.Selected).Select(z => z.Name).ToList());
    

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}