using Hydra.ViewModels.Commands;
using Hydra.ViewModels.PopUps;
using Hydra.Views.PopUps;
using Styx;
using Styx.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
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
    public DataTable DataTable { get; set; }

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


    private DataView displayData;
    public DataView DisplayData
    {
        get => displayData;
        set
        {
            displayData = value;
            OnPropertyChanged();
        }
    }


    private object? selectedObject;
    public object? SelectedObject
    {
        get => selectedObject;
        set
        {
            selectedObject = value;
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
    public ManageSiteCommand ManageSiteCommand { get; set; }
    public CustomizeLevelsCommand CustomizeLevelsCommand { get; set; }

    #endregion

    public ItemLevelsVM(HydraVM hydraVM)
    {
        HydraVM = hydraVM;
        DataSet = new HydraDataSet();
        AllItems = new List<NAVItem>();
        items = new ObservableCollection<NAVItem>();
        DataTable = new DataTable();
        displayData = new DataView();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        SelectItemsCommand = new SelectItemsCommand(this);
        SaveLevelsCommand = new SaveLevelsCommand(this);
        ManageSiteCommand = new ManageSiteCommand(this);
        CustomizeLevelsCommand = new CustomizeLevelsCommand(this);

        Task.Run(() => SetDataSources(HydraVM.Helios!, HydraVM.Charon!));
    }

    public void RefreshData()
    {
        if (Helios is null) return;
        Mouse.OverrideCursor = Cursors.Wait;
        DataSet = Helios.InventoryReader.HydraDataSet(false);
        AllItems = DataSet.Items.Values.ToList();
        Items = new ObservableCollection<NAVItem>(AllItems.Where(i => i.SiteLevelTarget));
        SetTables();

        ApplyFilters();
        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public void SetTables()
    {
        DataTable.Rows.Clear();
        DataTable.Columns.Clear();

        var column = new DataColumn
        {
            AutoIncrement = false,
            ColumnName = "Item",
            DataType = typeof(ItemVM),
        };

        DataTable.Columns.Add(column);

        foreach (var (_, site) in DataSet.Sites)
        {
            column = new DataColumn
            {
                AutoIncrement = false,
                ColumnName = site.Name,
                DataType = typeof(SiteItemLevel),
            };

            DataTable.Columns.Add(column);

        }

        foreach (var item in Items)
        {
            var row = DataTable.NewRow();
            row["Item"] = new ItemVM(item);

            foreach (var (_, site) in DataSet.Sites)
            {
                if (!DataSet.SiteItemLevels.TryGetValue((site.Name, item.Number), out var levels))
                    levels = new SiteItemLevel(item, site);
                row[site.Name] = levels;
            }
            DataTable.Rows.Add(row);
        }

        DisplayData = new DataView(DataTable);
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
        //DisplayData.RowFilter = "Item.Number = 155555";
    }

    public void ApplySorting()
    {
        throw new NotImplementedException();
    }

    public void SelectItems()
    {
        var vm = new ItemSelectionVM(this);
        var itemWindow = new ItemSelectionWindow(vm);
        if (itemWindow.ShowDialog() == true)
            RefreshData();
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

    public void ManageSite()
    {
        if (Helios is null || Charon is null || SelectedObject is null) return;
        var site = ((SiteItemLevel)SelectedObject).Site;
        if (site == null) return;

        var vm = new SiteManagementVM(this, site);
        var window = new SiteManagementWindow { DataContext = vm };
        if (window.ShowDialog() == true)
            RefreshData();
    }

    public void CustomizeLevels()
    {
        if (Helios is null || Charon is null || SelectedObject is null) return;
        var siteItemLevel = (SiteItemLevel)SelectedObject;

        var vm = new LevelManagementVM(this, siteItemLevel);
        var window = new LevelManagementWindow { DataContext = vm };
        window.ShowDialog();
        OnPropertyChanged(nameof(siteItemLevel));
    }
}