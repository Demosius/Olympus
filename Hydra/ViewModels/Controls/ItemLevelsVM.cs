using Styx;
using Styx.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Hydra.ViewModels.Commands;
using Hydra.Views;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels.Controls;

public class ItemLevelsVM : INotifyPropertyChanged, IDBInteraction, IDataSource, IFilters
{
    public HydraVM HydraVM { get; set; }
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public HydraDataSet DataSet { get; set; }

    public List<NAVItem> AllItems { get; set; }

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
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public SelectItemsCommand SelectItemsCommand { get; set; }
    public SaveLevelsCommand SaveLevelsCommand { get; set; }

    #endregion

    public ItemLevelsVM(HydraVM hydraVM)
    {
        HydraVM = hydraVM;
        DataSet = new HydraDataSet();
        AllItems = new List<NAVItem>();
        items = new ObservableCollection<NAVItem>();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        SelectItemsCommand = new SelectItemsCommand(this);
        SaveLevelsCommand = new SaveLevelsCommand(this);

        Task.Run(() => SetDataSources(HydraVM.Helios!, HydraVM.Charon!));
    }

    public void RefreshData()
    {
        if (Helios is null) return;

        DataSet = Helios.InventoryReader.HydraDataSet(false);
        AllItems = DataSet.Items.Values.ToList();
        Items = new ObservableCollection<NAVItem>(AllItems.Where(i => i.SiteLevelTarget));

        ApplyFilters();
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

    public void ClearFilters()
    {
        throw new NotImplementedException();
    }

    public void ApplyFilters()
    {
        // TODO: Implement.
    }

    public void ApplySorting()
    {
        throw new NotImplementedException();
    }

    public void SelectItems()
    {
        var vm = new ItemSelectionVM(this);
        var itemWindow = new ItemSelectionWindow(vm);
        itemWindow.ShowDialog();
    }

    public void SaveLevels()
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