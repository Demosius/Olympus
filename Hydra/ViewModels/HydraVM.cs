using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Hydra.ViewModels.Controls;
using Styx;
using Styx.Interfaces;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Hydra.ViewModels;

public class HydraVM : INotifyPropertyChanged, IDBInteraction, IDataSource
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public RunVM RunVM { get; set; }
    public SiteManagerVM SiteManagerVM { get; set; }
    public ZoneHandlerVM ZoneHandlerVM { get; set; }
    public ItemLevelsVM ItemLevelsVM { get; set; }

    #region INotifyPropertyChanged Members



    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }

    #endregion

    public HydraVM(Helios helios, Charon charon)
    {
        SetDataSources(helios, charon);
        RunVM = new RunVM(this);
        SiteManagerVM = new SiteManagerVM(this);
        ZoneHandlerVM = new ZoneHandlerVM(this);
        ItemLevelsVM = new ItemLevelsVM(this);

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
    }

    public void RefreshData()
    {
        RunVM.RefreshData();
        SiteManagerVM.RefreshData();
        ZoneHandlerVM.RefreshData();
    }

    public void RepairData()
    {
        throw new NotImplementedException();
    }

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