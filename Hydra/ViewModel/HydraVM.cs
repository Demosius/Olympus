﻿using Styx;
using Styx.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Hydra.ViewModel.Controls;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Hydra.ViewModel;

public class HydraVM : INotifyPropertyChanged, IDBInteraction, IDataSource
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public RunVM RunVM { get; set; }
    public SiteManagerVM SiteManagerVM { get; set; }
    public ZoneHandlerVM ZoneHandlerVM { get; set; }

    #region INotifyPropertyChanged Members



    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }

    #endregion

    public HydraVM()
    {
        RunVM = new RunVM(this);
        SiteManagerVM = new SiteManagerVM(this);
        ZoneHandlerVM = new ZoneHandlerVM(this);

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
    }

    public void RefreshData()
    {
        throw new NotImplementedException();
    }

    public void RepairData()
    {
        throw new NotImplementedException();
    }

    public void SetDataSources(Helios helios, Charon charon)
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