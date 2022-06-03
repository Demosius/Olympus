using Hydra.Interfaces;
using Hydra.ViewModels.Commands;
using Hydra.ViewModels.Controls;
using Styx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels.PopUps;

public class SiteManagementVM : IItemDataVM
{
    public ItemLevelsVM ItemLevelsVM { get; set; }
    public Site Site { get; set; }
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public List<SiteItemLevelVM> AllItems { get; set; }

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

    private ObservableCollection<SiteItemLevelVM> currentItems;
    public ObservableCollection<SiteItemLevelVM> CurrentItems
    {
        get => currentItems;
        set
        {
            currentItems = value;
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
    public FilterItemsFromClipboardCommand FilterItemsFromClipboardCommand { get; set; }
    public ActivateAllItemsCommand ActivateAllItemsCommand { get; set; }
    public DeActivateAllItemsCommand DeActivateAllItemsCommand { get; set; }
    public ExclusiveItemActivationCommand ExclusiveItemActivationCommand { get; set; }

    #endregion

    public SiteManagementVM(ItemLevelsVM parentVM, Site site)
    {
        ItemLevelsVM = parentVM;
        Site = site;
        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        FilterItemsFromClipboardCommand = new FilterItemsFromClipboardCommand(this);
        ActivateAllItemsCommand = new ActivateAllItemsCommand(this);
        DeActivateAllItemsCommand = new DeActivateAllItemsCommand(this);
        ExclusiveItemActivationCommand = new ExclusiveItemActivationCommand(this);

        AllItems = new List<SiteItemLevelVM>();
        filterString = string.Empty;
        currentItems = new ObservableCollection<SiteItemLevelVM>();

        SetDataSources(ItemLevelsVM.Helios!, ItemLevelsVM.Charon!);
    }


    public void RefreshData()
    {
        // TODO: Implement this thing!
        // Get hydra-active item's SiteItemLevels 
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
        throw new NotImplementedException();
    }

    public void ApplySorting()
    {
        throw new NotImplementedException();
    }

    public void FilterItemsFromClipboard()
    {
        throw new NotImplementedException();
    }

    public void ActivateAllItems()
    {
        throw new NotImplementedException();
    }

    public void DeActivateAllItems()
    {
        throw new NotImplementedException();
    }

    public void ExclusiveItemActivation()
    {
        throw new NotImplementedException();
    }

    public void ConfirmSiteChanges()
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