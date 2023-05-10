﻿using Morpheus.ViewModels.Commands;
using Serilog;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Morpheus.ViewModels.Controls;

public class ZoneHandlerVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }
    public Charon? Charon { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<NAVZone> Zones { get; set; }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public UploadZonesCommand UpdateZonesCommand { get; set; }
    public SaveZonesCommand SaveZonesCommand { get; set; }

    #endregion

    public ZoneHandlerVM(Helios helios, Charon? charon)
    {
        Helios = helios;
        Charon = charon;
        Zones = new ObservableCollection<NAVZone>();

        RefreshDataCommand = new RefreshDataCommand(this);
        UpdateZonesCommand = new UploadZonesCommand(this);
        SaveZonesCommand = new SaveZonesCommand(this);
        Task.Run(RefreshDataAsync);
    }

    public async Task RefreshDataAsync()
    {
        Zones.Clear();

        var zones = (await Helios.InventoryReader.ZonesAsync()).OrderBy(z => z.Code);
        foreach (var zone in zones) Zones.Add(zone);
        OnPropertyChanged(nameof(Zones));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async Task UploadZones()
    {
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
        await Helios.InventoryUpdater.ReplaceZonesAsync(newZones);

        await RefreshDataAsync();
    }

    public async Task SaveZones()
    {
        // Confirm with user.
        if (MessageBox.Show("Are you sure you want to save the changes made to the zones?",
                "Confirm Changes", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

        // Update/Replace table.
        await Helios.InventoryUpdater.ReplaceZonesAsync(Zones);
    }
}