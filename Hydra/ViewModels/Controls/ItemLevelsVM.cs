using Styx;
using Styx.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels.Controls;

public class ItemLevelsVM : INotifyPropertyChanged, IDBInteraction, IDataSource
{
    public HydraVM HydraVM { get; set; }
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public HydraDataSet DataSet { get; set; }

    private List<NAVItem> allItems;

    #region INotifyPropertyChanged Members

    private ObservableCollection<NAVItem> items;
    public ObservableCollection<NAVItem> Items
    {
        get => items;
        set
        {
            items = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }

    #endregion

    public ItemLevelsVM(HydraVM hydraVM)
    {
        HydraVM = hydraVM;
        DataSet = new HydraDataSet();
        allItems = new List<NAVItem>();
        items = new ObservableCollection<NAVItem>();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);

        Task.Run(() => SetDataSources(HydraVM.Helios!, HydraVM.Charon!));
    }

    public void RefreshData()
    {
        if (Helios is null) return;

        DataSet = Helios.InventoryReader.HydraDataSet(false);
        allItems = DataSet.Items.Values.ToList();
        Items = new ObservableCollection<NAVItem>(allItems.Where(i => i.SiteLevelTarget));
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
}