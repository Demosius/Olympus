using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Morpheus.ViewModels.Commands;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Morpheus.ViewModels.Windows;

public class ItemSelectionVM : INotifyPropertyChanged, IFilters
{
    public List<NAVItem> Items { get; set; }
    public List<NAVItem> SelectedItems { get; set; }
    
    #region INotifyPropertyChanged Members
    
    public ObservableCollection<NAVItem> FilteredItems { get; set; }

    private string itemFilter;
    public string ItemFilter
    {
        get => itemFilter;
        set
        {
            itemFilter = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private NAVItem? selectedItem;
    public NAVItem? SelectedItem
    {
        get => selectedItem;
        set
        {
            selectedItem = value;
            OnPropertyChanged();
        }
    }

    private SelectionMode selectionMode;
    public SelectionMode SelectionMode
    {
        get => selectionMode;
        set
        {
            selectionMode = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ConfirmInputCommand ConfirmInputCommand { get; set; }

    #endregion

    public ItemSelectionVM(List<NAVItem> items, SelectionMode selectMode = SelectionMode.Single)
    {
        Items = items;
        SelectionMode = selectMode;
        FilteredItems = new ObservableCollection<NAVItem>(Items);
        SelectedItems = new List<NAVItem>();
        itemFilter = string.Empty;

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ConfirmInputCommand = new ConfirmInputCommand();
    }

    public void ClearFilters()
    {
        FilteredItems.Clear();
        itemFilter = string.Empty;
    }

    public void ApplyFilters()
    {
        var items = Items.Where(i =>
            Regex.IsMatch(i.Number.ToString(format: "000000"), ItemFilter, RegexOptions.IgnoreCase));

        FilteredItems.Clear();
        foreach (var item in items)
            FilteredItems.Add(item);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}