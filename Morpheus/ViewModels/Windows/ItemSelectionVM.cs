using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Morpheus.ViewModels.Commands;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Morpheus.ViewModels.Windows;

public class ItemSelectionVM : INotifyPropertyChanged, IFilters
{
    public List<NAVItem> Items { get; set; }
    
    #region INotifyPropertyChanged Members

    private ObservableCollection<NAVItem> filteredItems;
    public ObservableCollection<NAVItem> FilteredItems
    {
        get => filteredItems;
        set
        {
            filteredItems = value;
            OnPropertyChanged();
        }
    }

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


    #endregion

    #region Commands

    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ConfirmInputCommand ConfirmInputCommand { get; set; }

    #endregion

    public ItemSelectionVM(List<NAVItem> items)
    {
        Items = items;
        filteredItems = new ObservableCollection<NAVItem>(Items);
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
        var regex = new Regex(ItemFilter);
        FilteredItems = new ObservableCollection<NAVItem>(Items.Where(i => regex.IsMatch(i.Number.ToString(format: "000000"))));
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}