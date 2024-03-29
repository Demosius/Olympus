﻿using Styx;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus;
using Uranus.Annotations;

namespace Prometheus.ViewModels.Pages;

internal class EquipmentVM : INotifyPropertyChanged
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    #region INotifyPropertyChanged Members



    #endregion

    public EquipmentVM() { }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}