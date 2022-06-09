using Hydra.Interfaces;
using Hydra.ViewModels.Commands;
using Hydra.ViewModels.Controls;
using Styx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels.PopUps;

public class SiteManagementVM : INotifyPropertyChanged, IItemDataVM
{
    public ItemLevelsVM ItemLevelsVM { get; set; }
    public SiteVM Site { get; set; }
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
    public ConfirmSiteChangesCommand ConfirmSiteChangesCommand { get; set; }

    #endregion

    public SiteManagementVM(ItemLevelsVM parentVM, Site site)
    {
        ItemLevelsVM = parentVM;
        Site = new SiteVM(site);

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        FilterItemsFromClipboardCommand = new FilterItemsFromClipboardCommand(this);
        ActivateAllItemsCommand = new ActivateAllItemsCommand(this);
        DeActivateAllItemsCommand = new DeActivateAllItemsCommand(this);
        ExclusiveItemActivationCommand = new ExclusiveItemActivationCommand(this);
        ConfirmSiteChangesCommand = new ConfirmSiteChangesCommand(this);

        AllItems = new List<SiteItemLevelVM>();
        filterString = string.Empty;
        currentItems = new ObservableCollection<SiteItemLevelVM>();

        SetDataSources(ItemLevelsVM.Helios!, ItemLevelsVM.Charon!);
    }


    public void RefreshData()
    {
        AllItems = ItemLevelsVM.SiteItemLevelVMs.Values
            .Where(sil => sil.SiteName == Site.Site.Name && (sil.Item?.SiteLevelTarget ?? false))
            .OrderBy(sil => sil.ItemNumber)
            .ToList();
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
        FilterString = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        var regex = new Regex(FilterString);
        CurrentItems =
            new ObservableCollection<SiteItemLevelVM>(AllItems.Where(sil => regex.IsMatch(sil.ItemNumber.ToString())));
    }

    public void ApplySorting()
    {
        throw new NotImplementedException();
    }

    public void FilterItemsFromClipboard()
    {
        Mouse.OverrideCursor = Cursors.Wait;
        var numbers = new List<int>();

        // Set data.
        var rawData = General.ClipboardToString();
        var byteArray = Encoding.UTF8.GetBytes(rawData);
        var stream = new MemoryStream(byteArray);
        using var reader = new StreamReader(stream);

        // Get the item number column, if there is one.
        var line = reader.ReadLine();
        var headArr = line?.Split('\t') ?? Array.Empty<string>();

        var itemIndex = Array.IndexOf(headArr, "Item No.");
        if (itemIndex == -1) itemIndex = Array.IndexOf(headArr, "Item Number");
        if (itemIndex == -1) itemIndex = Array.IndexOf(headArr, "Item");

        if (itemIndex == -1)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            MessageBox.Show("Could not detect item number values within clipboard data.");
            return;
        }

        line = reader.ReadLine();

        while (line is not null)
        {
            var row = line.Split('\t');

            if (int.TryParse(row[itemIndex], out var itemNumber)) numbers.Add(itemNumber);

            line = reader.ReadLine();
        }

        numbers.Sort();

        const int x = 3000;

        MessageBox.Show(numbers.Count <= x
            ? $"Found {numbers.Count:#,###} potential item numbers."
            : $"Found {numbers.Count:#,###} potential item numbers. Will only use the first {x:#,###}.");

        FilterString = string.Join("|", numbers.Select(n => n.ToString("000000")).Take(x));
        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public void ActivateAllItems()
    {
        foreach (var siteItemLevelVM in CurrentItems) siteItemLevelVM.Active = true;
    }

    public void DeActivateAllItems()
    {
        foreach (var siteItemLevelVM in CurrentItems) siteItemLevelVM.Active = false;
    }

    public void ExclusiveItemActivation()
    {
        foreach (var siteItemLevelVM in AllItems) siteItemLevelVM.Active = false;
        foreach (var siteItemLevelVM in CurrentItems) siteItemLevelVM.Active = true;
    }

    public void ConfirmSiteChanges()
    {
        if (Helios is null) return;

        Mouse.OverrideCursor = Cursors.Wait;
        Helios.InventoryUpdater.SiteItemLevels(AllItems.Select(vm => vm.SiteItemLevel));
        Helios.InventoryUpdater.Site(Site.Site);
        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}