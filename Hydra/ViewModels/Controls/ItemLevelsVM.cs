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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels.Controls;

public enum EItemLevelFilter
{
    None,
    AllActive,
    AnyActive,
    AnyInactive,
    AllInactive,
    Custom
}

public class ItemLevelsVM : INotifyPropertyChanged, IDBInteraction, IDataSource, IFilters
{
    public HydraVM HydraVM { get; set; }
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public HydraDataSet? DataSet { get; set; }

    public List<ItemVM> AllItems { get; set; }
    public DataTable DataTable { get; set; }

    public Dictionary<(string, int), SiteItemLevelVM> SiteItemLevelVMs { get; set; }
    public List<SiteVM> Sites { get; set; }

    #region INotifyPropertyChanged Members

    private ObservableCollection<ItemVM> items;
    public ObservableCollection<ItemVM> Items
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

    private string filterString;
    public string FilterString
    {
        get => filterString;
        set
        {
            filterString = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }


    private EItemLevelFilter selectedFilter;
    public EItemLevelFilter SelectedFilter
    {
        get => selectedFilter;
        set
        {
            selectedFilter = value;
            OnPropertyChanged();
            ApplyFilters();
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
        AllItems = new List<ItemVM>();
        items = new ObservableCollection<ItemVM>();
        DataTable = new DataTable();
        displayData = new DataView();
        SiteItemLevelVMs = new Dictionary<(string, int), SiteItemLevelVM>();
        Sites = new List<SiteVM>();
        filterString = string.Empty;

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

        SetVMs();

        Items = new ObservableCollection<ItemVM>(AllItems.Where(i => i.UseLevelTargets));
        SetTables();

        ApplyFilters();
        Mouse.OverrideCursor = Cursors.Arrow;
        OnPropertyChanged(nameof(DisplayData));
    }

    private void SetVMs()
    {
        SiteItemLevelVMs = DataSet.SiteItemLevels.Values.Select(sil => new SiteItemLevelVM(sil))
            .ToDictionary(sil => (sil.SiteName, sil.ItemNumber), sil => sil);
        Sites = DataSet.Sites.Values.Select(s => new SiteVM(s)).ToList();
        AllItems = DataSet.Items.Values.Select(s => new ItemVM(s)).ToList();
        foreach (var itemVM in AllItems)
        {
            foreach (var siteVM in Sites)
            {
                if (!SiteItemLevelVMs.TryGetValue((siteVM.Name, itemVM.Item.Number), out var sil))
                    sil = new SiteItemLevelVM(new SiteItemLevel(itemVM.Item, siteVM.Site));

                sil.SiteVM = siteVM;
                sil.ItemVM = itemVM;
                siteVM.ItemLevelVMs.Add(sil);
                itemVM.SiteLevelVMs.Add(sil);
            }
        }
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
                DataType = typeof(SiteItemLevelVM),
            };

            DataTable.Columns.Add(column);

        }

        foreach (var item in Items)
        {
            var row = DataTable.NewRow();
            row["Item"] = item;

            foreach (var (_, site) in DataSet.Sites)
            {
                if (!SiteItemLevelVMs.TryGetValue((site.Name, item.Item.Number), out var levels))
                {
                    levels = new SiteItemLevelVM(new SiteItemLevel(item.Item, site));
                    SiteItemLevelVMs.Add((levels.SiteName, levels.ItemNumber), levels);
                }
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
        filterString = string.Empty;
        selectedFilter = EItemLevelFilter.None;
        OnPropertyChanged(nameof(FilterString));
        OnPropertyChanged(nameof(SelectedFilter));
        DisplayData = DataTable.DefaultView;
    }

    public void ApplyFilters()
    {
        var regex = new Regex(FilterString);

        var dataRows = DataTable
            .AsEnumerable()
            .Where(dataRow => regex.IsMatch(dataRow.Field<ItemVM>("Item")?.Number.ToString() ?? ""));

        switch (SelectedFilter)
        {
            case EItemLevelFilter.None:
                break;
            case EItemLevelFilter.AllActive:
                dataRows = dataRows.Where(r =>
                    r.Field<ItemVM>("Item")?.SiteLevelVMs.All(siteLevels => siteLevels.Active) ?? false);
                break;
            case EItemLevelFilter.AnyActive:
                dataRows = dataRows.Where(r =>
                    r.Field<ItemVM>("Item")?.SiteLevelVMs.Any(siteLevels => siteLevels.Active) ?? false);
                break;
            case EItemLevelFilter.AnyInactive:
                dataRows = dataRows.Where(r =>
                    r.Field<ItemVM>("Item")?.SiteLevelVMs.Any(siteLevels => !siteLevels.Active) ?? false);
                break;
            case EItemLevelFilter.AllInactive:
                dataRows = dataRows.Where(r =>
                    r.Field<ItemVM>("Item")?.SiteLevelVMs.All(siteLevels => !siteLevels.Active) ?? false);
                break;
            case EItemLevelFilter.Custom:
                dataRows = dataRows.Where(r =>
                    r.Field<ItemVM>("Item")?.SiteLevelVMs
                        .Any(siteLevels => siteLevels.Active && siteLevels.OverrideDefaults) ?? false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        DisplayData = dataRows.AsDataView();
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
        if (Helios is null) return;

        Helios.InventoryUpdater.SiteItemLevels(DataSet.SiteItemLevels.Values.Where(sil => sil.Item?.SiteLevelTarget ?? false));
        MessageBox.Show("Successfully saved Site Item Levels data.");
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
        var site = ((SiteItemLevelVM)SelectedObject).Site;
        if (site == null) return;

        var vm = new SiteManagementVM(this, site);
        var window = new SiteManagementWindow { DataContext = vm };
        if (window.ShowDialog() == true)
            RefreshData();
    }

    public void CustomizeLevels()
    {
        if (Helios is null || Charon is null || SelectedObject is null) return;
        var siteItemLevel = (SiteItemLevelVM)SelectedObject;

        var vm = new LevelManagementVM(this, siteItemLevel);
        var window = new LevelManagementWindow { DataContext = vm };
        window.ShowDialog();
        ApplyFilters();
        SelectedObject = siteItemLevel;
    }
}