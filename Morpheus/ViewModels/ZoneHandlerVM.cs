using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Morpheus.ViewModels.Commands;
using Serilog;
using Styx;
using Styx.Interfaces;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Morpheus.ViewModels;

public class ZoneHandlerVM : INotifyPropertyChanged, IDBInteraction, IDataSource
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<NAVZone> Zones { get; set; }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }
    public UploadZonesCommand UpdateZonesCommand { get; set; }
    public SaveZonesCommand SaveZonesCommand { get; set; }

    #endregion

    public ZoneHandlerVM(Helios helios, Charon? charon)
    {
        Zones = new ObservableCollection<NAVZone>();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        UpdateZonesCommand = new UploadZonesCommand(this);
        SaveZonesCommand = new SaveZonesCommand(this);
        Task.Run(() => SetDataSources(helios, charon!));
    }

    public void RefreshData()
    {
        if (Helios is null) return;
        Zones.Clear();

        var zones = Helios.InventoryReader.Zones().OrderBy(z => z.Code);
        foreach (var zone in zones) Zones.Add(zone);
        OnPropertyChanged(nameof(zones));
    }

    public void RepairData()
    {
        throw new NotImplementedException();
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        RefreshData();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void UploadZones()
    {
        if (Helios is null) return;

        // Get data sets and check data validity.
        List<NAVZone>? newZones;
        try
        {
            newZones = DataConversion.NAVRawStringToZones(General.ClipboardToString());
        }
        catch (InvalidDataException ex)
        {
            Log.Error(ex, "Invalid data for NAV zone update.");
            MessageBox.Show("Invalid data fond on the clipboard.", "Invalid Data Exception Thrown", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!newZones.Any()) return;

        // Check data difference. Count: Match, To-be-deleted(only current), & To-be-added(only new).
        var matchCount = newZones.Count(z => Zones.Contains(z));
        var delCount = Zones.Count(z => !newZones.Contains(z));
        var newCount = newZones.Count(z => !Zones.Contains(z));

        // Confirm replacement.
        var result = MessageBox.Show($"{delCount} lines deleted.\n" +
                                     $"{matchCount} lines updated.\n" +
                                     $"{newCount} lines added.\n" +
                                     "Would you like to continue with the update?", "Confirm Update",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) return;

        // Apply update/replacement.
        Helios.InventoryUpdater.ReplaceZones(newZones);

        RefreshData();
    }

    public void SaveZones()
    {
        // Confirm with user.
        if (Helios is null || MessageBox.Show("Are you sure you want to save the changes made to the zones?",
                "Confirm Changes", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

        // Update/Replace table.
        Helios.InventoryUpdater.ReplaceZones(Zones);

    }
}