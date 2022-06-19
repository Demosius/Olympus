using Hydra.Helpers;
using Hydra.Interfaces;
using Hydra.ViewModels.Commands;
using Styx;
using Styx.Interfaces;
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

namespace Hydra.ViewModels.Controls;

public class RunVM : INotifyPropertyChanged, IDBInteraction, IDataSource, IItemFilters
{
    public HydraVM HydraVM { get; set; }
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public ObservableCollection<SiteVM> Sites { get; set; }

    public List<MoveVM> AllMoves { get; set; }

    #region InotifyPropertyChanged Members

    private ObservableCollection<MoveVM> currentMoves;
    public ObservableCollection<MoveVM> CurrentMoves
    {
        get => currentMoves;
        set
        {
            currentMoves = value;
            OnPropertyChanged();
        }
    }

    private string itemFilterString;
    public string ItemFilterString
    {
        get => itemFilterString;
        set
        {
            itemFilterString = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }


    private string fromSiteFilterString;
    public string FromSiteFilterString
    {
        get => fromSiteFilterString;
        set
        {
            fromSiteFilterString = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }


    private string toSiteFilterString;
    public string ToSiteFilterString
    {
        get => toSiteFilterString;
        set
        {
            toSiteFilterString = value;
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
    public FilterItemsFromClipboardCommand FilterItemsFromClipboardCommand { get; set; }
    public GenerateMovesCommand GenerateMovesCommand { get; set; }

    #endregion

    public RunVM(HydraVM hydraVM)
    {
        HydraVM = hydraVM;
        Sites = new ObservableCollection<SiteVM>();
        AllMoves = new List<MoveVM>();
        currentMoves = new ObservableCollection<MoveVM>();

        itemFilterString = string.Empty;
        fromSiteFilterString = string.Empty;
        toSiteFilterString = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        FilterItemsFromClipboardCommand = new FilterItemsFromClipboardCommand(this);
        GenerateMovesCommand = new GenerateMovesCommand(this);

        Task.Run(() => SetDataSources(HydraVM.Helios!, HydraVM.Charon!));
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        RefreshData();
    }

    public void RefreshData()
    {
        if (Helios is null) return;

        Sites.Clear();
        foreach (var site in Helios.InventoryReader.Sites(out _))
        {
            Sites.Add(new SiteVM(site));
        }
    }

    public void RepairData()
    {
        throw new NotImplementedException();
    }

    public void ClearFilters()
    {
        itemFilterString = string.Empty;
        fromSiteFilterString = string.Empty;
        toSiteFilterString = string.Empty;
        CurrentMoves = new ObservableCollection<MoveVM>(AllMoves);
        OnPropertyChanged(nameof(ItemFilterString));
        OnPropertyChanged(nameof(FromSiteFilterString));
        OnPropertyChanged(nameof(ToSiteFilterString));
    }

    public void ApplyFilters()
    {
        var itemRegex = new Regex(ItemFilterString);
        var fromRegex = new Regex(FromSiteFilterString);
        var toRegex = new Regex(ToSiteFilterString);

        CurrentMoves = new ObservableCollection<MoveVM>(AllMoves.Where(m =>
            itemRegex.IsMatch(m.ItemNumber.ToString()) &&
            fromRegex.IsMatch(m.TakeSiteName) &&
            toRegex.IsMatch(m.PlaceSiteName)));
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

        ItemFilterString = string.Join("|", numbers.Select(n => n.ToString("000000")).Take(x));
        Mouse.OverrideCursor = Cursors.Arrow;
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

    public void GenerateMoves()
    {
        if (Helios is null) return;

        Mouse.OverrideCursor = Cursors.Wait;
        var hds = Helios.InventoryReader.HydraDataSet();
        var siteMoves =
            MoveGenerator.GenerateSiteMoves(hds, Sites.Where(s => s.TakeFrom).Select(s => s.Name),
                Sites.Where(s => s.PlaceTo).Select(s => s.Name));

        AllMoves = siteMoves.Select(m => new MoveVM(m)).ToList();
        ApplyFilters();
        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}