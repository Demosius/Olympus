using Hydra.Interfaces;
using Hydra.ViewModels.Commands;
using Hydra.ViewModels.Controls;
using Morpheus;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Hydra.ViewModels.PopUps;

public class ItemSelectionVM : INotifyPropertyChanged, IItemDataVM, ISorting
{
    public ItemLevelsVM ItemLevelsVM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

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

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
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
        Helios = vm.Helios;
        Charon = vm.Charon;

        RefreshDataCommand = new RefreshDataCommand(this);
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

        Task.Run(RefreshDataAsync);
    }

    public async Task RefreshDataAsync()
    {
        await new Task(() =>
        {
            AllItems = ItemLevelsVM.AllItems;
            ApplyFilters();
        });
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

    public async Task ConfirmItemSelection()
    {
        ItemLevelsVM.AllItems = AllItems;
        await Helios.InventoryUpdater.NAVItemsAsync(ItemLevelsVM.AllItems.Select(vm => vm.Item), DateTime.Now);
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