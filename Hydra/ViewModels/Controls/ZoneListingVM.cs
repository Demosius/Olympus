using Hydra.ViewModels.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels.Controls;

public class ZoneListingVM : INotifyPropertyChanged
{
    public SiteVM? Site { get; set; }

    public ObservableCollection<ZoneVM> ZoneVMs { get; set; }

    private ZoneVM? incomingZoneVM;
    public ZoneVM? IncomingZoneVM
    {
        get => incomingZoneVM;
        set
        {
            incomingZoneVM = value;
            OnPropertyChanged(nameof(IncomingZoneVM));
        }
    }

    private ZoneVM? removedZoneVM;
    public ZoneVM? RemovedZoneVM
    {
        get => removedZoneVM;
        set
        {
            removedZoneVM = value;
            OnPropertyChanged(nameof(RemovedZoneVM));
        }
    }

    private ZoneVM? insertedZoneVM;
    public ZoneVM? InsertedZoneVM
    {
        get => insertedZoneVM;
        set
        {
            insertedZoneVM = value;
            OnPropertyChanged(nameof(InsertedZoneVM));
        }
    }

    private ZoneVM? targetZoneVM;
    public ZoneVM? TargetZoneVM
    {
        get => targetZoneVM;
        set
        {
            targetZoneVM = value;
            OnPropertyChanged(nameof(TargetZoneVM));
        }
    }

    public ICommand ZoneReceivedCommand { get; }
    public ICommand ZoneRemovedCommand { get; }
    public ICommand ZoneInsertedCommand { get; }

    public ZoneListingVM(IEnumerable<NAVZone> zones, SiteVM? site = null)
    {
        ZoneVMs = new ObservableCollection<ZoneVM>(zones.Select(z => new ZoneVM(z)));
        foreach (var zoneVM in ZoneVMs)
        {
            zoneVM.SiteVM = site;
        }
        Site = site;

        ZoneReceivedCommand = new ZoneReceivedCommand(this);
        ZoneRemovedCommand = new ZoneRemovedCommand(this);
        ZoneInsertedCommand = new ZoneInsertedCommand(this);
    }

    public void AddZone(ZoneVM item)
    {
        if (ZoneVMs.Contains(item)) return;

        ZoneVMs.Add(item);
        item.Remove();
        item.SiteVM = Site;
        item.Add();
    }

    public void InsertZone(ZoneVM insertedTodoItem, ZoneVM targetTodoItem)
    {
        if (insertedTodoItem == targetTodoItem)
        {
            return;
        }

        var oldIndex = ZoneVMs.IndexOf(insertedTodoItem);
        var nextIndex = ZoneVMs.IndexOf(targetTodoItem);

        if (oldIndex != -1 && nextIndex != -1)
        {
            ZoneVMs.Move(oldIndex, nextIndex);
        }
    }

    public void RemoveZone(ZoneVM item)
    {
        ZoneVMs.Remove(item);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}