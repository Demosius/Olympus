using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Morpheus.Views.Controls;
using Uranus;
using Uranus.Annotations;

namespace Prometheus.ViewModels.Pages;

internal class InventoryVM : INotifyPropertyChanged
{
    public enum EInventoryControl
    {
        Zones,
        Bins,
        Stock,
        Items,
        Bays,
        Categories,
        Divisions,
        Platforms,
        Genres,
        Sites,
        Stores,
        TransferOrders,
        MixedCartons
    }

    public Helios? Helios { get; set; }

    public Dictionary<EInventoryControl, Control?> ControlDict { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<EInventoryControl> Controls { get; set; }

    private EInventoryControl? selectedControl;
    public EInventoryControl? SelectedControl
    {
        get => selectedControl;
        set
        {
            selectedControl = value;
            OnPropertyChanged();
            SetControl(selectedControl);
        }
    }

    private Control? currentControl;
    public Control? CurrentControl
    {
        get => currentControl;
        set
        {
            currentControl = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public InventoryVM(Helios helios)
    {
        Helios = helios;

        ControlDict = new Dictionary<EInventoryControl, Control?>();

        Controls = new ObservableCollection<EInventoryControl>
        {
            EInventoryControl.Bays,
            EInventoryControl.Categories,
            EInventoryControl.Divisions,
            EInventoryControl.Platforms,
            EInventoryControl.Genres,
            EInventoryControl.Sites,
            EInventoryControl.Stores,
            EInventoryControl.TransferOrders,
            EInventoryControl.Bins,
            EInventoryControl.Zones,
            EInventoryControl.Items,
            EInventoryControl.Stock,
            EInventoryControl.MixedCartons
        };
    }

    public void SetControl(EInventoryControl? control)
    {
        if (Helios is null || control is null) return;

        var valueControl = (EInventoryControl)control;

        if (!ControlDict.TryGetValue(valueControl, out var newControl))
        {
            newControl = valueControl switch
            {
                EInventoryControl.Bays => currentControl,
                EInventoryControl.Bins => currentControl,
                EInventoryControl.Categories => currentControl,
                EInventoryControl.Divisions => currentControl,
                EInventoryControl.Genres => currentControl,
                EInventoryControl.Items => currentControl,
                EInventoryControl.Platforms => currentControl,
                EInventoryControl.Sites => currentControl,
                EInventoryControl.Stock => CurrentControl,
                EInventoryControl.Stores => currentControl,
                EInventoryControl.TransferOrders => currentControl,
                EInventoryControl.Zones => new ZoneHandlerView(Helios, null),
                EInventoryControl.MixedCartons => new MixedCartonHandlerView(Helios),
                _ => currentControl
            };
            ControlDict.Add(valueControl, newControl);
        }

        CurrentControl = newControl;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}