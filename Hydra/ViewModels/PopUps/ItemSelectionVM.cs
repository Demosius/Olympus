using Hydra.Interfaces;
using Hydra.ViewModels.Commands;
using Hydra.ViewModels.Controls;
using Styx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;

namespace Hydra.ViewModels.PopUps;

public class ItemSelectionVM : INotifyPropertyChanged, IItemDataVM
{
    public ItemLevelsVM ItemLevelsVM { get; set; }
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public List<ItemVM> AllItems { get; set; }

    #region INotifyPropertChanged Members


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

    private ObservableCollection<ItemVM> currentItems;
    public ObservableCollection<ItemVM> CurrentItems
    {
        get => currentItems;
        set
        {
            currentItems = value;
            OnPropertyChanged();
        }
    }


    private DataTable tempTable;
    public DataTable TempTable
    {
        get => tempTable;
        set
        {
            tempTable = value;
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
    public ConfirmItemSelectionCommand ConfirmItemSelectionCommand { get; set; }
    public FilterItemsFromClipboardCommand FilterItemsFromClipboardCommand { get; set; }
    public ActivateAllItemsCommand ActivateAllItemsCommand { get; set; }
    public DeActivateAllItemsCommand DeActivateAllItemsCommand { get; set; }
    public ExclusiveItemActivationCommand ExclusiveItemActivationCommand { get; set; }

    #endregion

    public ItemSelectionVM(ItemLevelsVM vm)
    {
        ItemLevelsVM = vm;

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        ConfirmItemSelectionCommand = new ConfirmItemSelectionCommand(this);
        FilterItemsFromClipboardCommand = new FilterItemsFromClipboardCommand(this);
        ActivateAllItemsCommand = new ActivateAllItemsCommand(this);
        DeActivateAllItemsCommand = new DeActivateAllItemsCommand(this);
        ExclusiveItemActivationCommand = new ExclusiveItemActivationCommand(this);

        filterString = string.Empty;
        AllItems = new List<ItemVM>();
        currentItems = new ObservableCollection<ItemVM>();

        SetDataSources(vm.Helios!, vm.Charon!);

        tempTable = new DataTable();
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        RefreshData();
    }

    public void RefreshData()
    {
        AllItems = ItemLevelsVM.AllItems.Select(i => new ItemVM(i)).ToList();
        ApplyFilters();
    }

    public void RepairData()
    {
        throw new NotImplementedException();
    }

    public void ClearFilters()
    {
        FilterString = string.Empty;
    }

    public void ApplyFilters()
    {
        var regex = new Regex(FilterString);
        CurrentItems = new ObservableCollection<ItemVM>(AllItems.Where(i => regex.IsMatch(i.Number)));
    }

    public void ApplySorting()
    {
        CurrentItems = new ObservableCollection<ItemVM>(CurrentItems.OrderBy(i => i.Number));
    }


    public void ConfirmItemSelection()
    {
        if (Helios is null) return;
        ItemLevelsVM.AllItems = AllItems.Select(i => i.Item).ToList();
        Helios.InventoryUpdater.NAVItems(ItemLevelsVM.AllItems, DateTime.Now);
    }

    public void FilterItemsFromClipboard()
    {
        // TODO: THIS
        var table = DataConversion.RawStringToTable(General.ClipboardToString());
        var numbers = new List<int>();
        foreach (DataColumn column in table.Columns)
        {
            if (column.ColumnName == "Item No.")
            {
                foreach (DataRow row in table.Rows)
                {
                    //numbers.Add((int)row[column]);
                }
            }
        }

        MessageBox.Show($"Found {numbers.Count} potential item numbers.");
        TempTable = table;
    }

    public void ActivateAllItems()
    {
        foreach (var item in CurrentItems) item.UseLevelTargets = true;
    }

    public void DeActivateAllItems()
    {
        foreach (var item in CurrentItems) item.UseLevelTargets = false;
    }

    public void ExclusiveItemActivation()
    {
        foreach (var item in AllItems) item.UseLevelTargets = false;
        foreach (var item in CurrentItems) item.UseLevelTargets = true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}