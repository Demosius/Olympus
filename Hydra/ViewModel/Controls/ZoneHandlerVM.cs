using System.Collections.ObjectModel;
using Styx;
using Styx.Interfaces;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Model;

namespace Hydra.ViewModel.Controls;

public class ZoneHandlerVM : INotifyPropertyChanged, IDBInteraction, IDataSource
{
    public HydraVM HydraVM { get; set; }
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<NAVZone> Zones { get; set; }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }

    #endregion

    public ZoneHandlerVM(HydraVM hydraVM)
    {
        HydraVM = hydraVM;
        Zones = new ObservableCollection<NAVZone>();
        
        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        Task.Run(() => SetDataSources(HydraVM.Helios!, HydraVM.Charon!));
    }

    public void RefreshData()
    {
        if (Helios is null) return;
        Zones.Clear();

        var zones = Helios.InventoryReader.Zones().OrderBy(z => z.Code);
        foreach (var zone in zones) Zones.Add(zone);
    }

    public void RepairData()
    {
        throw new System.NotImplementedException();
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

    public void UpdateZones()
    {
        if (Helios is null) return;

        Helios.InventoryUpdater.Zones(Zones);
    }
}