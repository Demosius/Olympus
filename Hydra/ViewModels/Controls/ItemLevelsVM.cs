﻿using Hydra.ViewModels.Commands;
using Hydra.ViewModels.PopUps;
using Hydra.Views.PopUps;
using Styx;
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

public class ItemLevelsVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public HydraVM HydraVM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public HydraDataSet DataSet { get; set; }

    public List<ItemVM> AllItems { get; set; }
    public DataTable DataTable { get; set; }

    public Dictionary<(string, int), SiteItemLevelVM> SiteItemLevelVMs { get; set; }
    public List<SiteVM> Sites { get; set; }

    #region INotifyPropertyChanged Members
    
    public ObservableCollection<ItemVM> Items { get; set; }


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
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public SelectItemsCommand SelectItemsCommand { get; set; }
    public SaveLevelsCommand SaveLevelsCommand { get; set; }
    public ManageSiteCommand ManageSiteCommand { get; set; }
    public CustomizeLevelsCommand CustomizeLevelsCommand { get; set; }

    #endregion

    private ItemLevelsVM(HydraVM hydraVM)
    {
        HydraVM = hydraVM;

        Helios = hydraVM.Helios;
        Charon = hydraVM.Charon;

        DataSet = new HydraDataSet();
        AllItems = new List<ItemVM>();
        Items = new ObservableCollection<ItemVM>();
        DataTable = new DataTable();
        displayData = new DataView();
        SiteItemLevelVMs = new Dictionary<(string, int), SiteItemLevelVM>();
        Sites = new List<SiteVM>();
        filterString = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        SelectItemsCommand = new SelectItemsCommand(this);
        SaveLevelsCommand = new SaveLevelsCommand(this);
        ManageSiteCommand = new ManageSiteCommand(this);
        CustomizeLevelsCommand = new CustomizeLevelsCommand(this);
    }

    public ItemLevelsVM(HydraVM hydraVM, HydraDataSet dataSet) : this(hydraVM)
    {
        DataSet = dataSet;

        SetVMs();

        var items = new List<ItemVM>(AllItems.Where(i => i.UseLevelTargets));
        Items.Clear();
        foreach (var item in items)
            Items.Add(item);

        SetTables();

        ApplyFilters();
    }

    private async Task<ItemLevelsVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<ItemLevelsVM> CreateAsync(HydraVM hydraVM)
    {
        var ret = new ItemLevelsVM(hydraVM);
        return ret.InitializeAsync();
    }

    public async Task RefreshDataAsync()
    {
        Mouse.OverrideCursor = Cursors.Wait;
        DataSet = await Helios.InventoryReader.HydraDataSetAsync(false) ?? new HydraDataSet();

        SetVMs();

        var items = new List<ItemVM>(AllItems.Where(i => i.UseLevelTargets));
        Items.Clear();
        foreach (var item in items)
            Items.Add(item);

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
    
    public async Task SelectItems()
    {
        var vm = await ItemSelectionVM.CreateAsync(this);
        var itemWindow = new ItemSelectionWindow(vm);
        if (itemWindow.ShowDialog() == true)
            await RefreshDataAsync();
    }

    public async Task SaveLevels()
    {
        await Helios.InventoryUpdater.SiteItemLevelsAsync(DataSet.SiteItemLevels.Values.Where(sil => sil.Item?.SiteLevelTarget ?? false));
        MessageBox.Show("Successfully saved Site Item Levels data.");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async Task ManageSite()
    {
        if (SelectedObject is null) return;
        var site = ((SiteItemLevelVM)SelectedObject).Site;
        if (site == null) return;

        var vm = await SiteManagementVM.CreateAsync(this, site);
        var window = new SiteManagementWindow { DataContext = vm };
        if (window.ShowDialog() == true)
            await RefreshDataAsync();
    }

    public void CustomizeLevels()
    {
        if (SelectedObject is null) return;
        var siteItemLevel = (SiteItemLevelVM)SelectedObject;

        var vm = new LevelManagementVM(this, siteItemLevel);
        var window = new LevelManagementWindow { DataContext = vm };
        window.ShowDialog();
        ApplyFilters();
        SelectedObject = siteItemLevel;
    }
}