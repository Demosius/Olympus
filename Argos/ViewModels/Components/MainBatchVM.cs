using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Argos.ViewModels.Commands;
using Morpheus;
using Morpheus.Views.Windows;
using Serilog;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Argos.ViewModels.Components;

public class MainBatchVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public Helios Helios { get; set; }

    public List<BatchVM> AllBatches { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<BatchVM> Batches { get; set; }
    public ObservableCollection<BatchVM> SelectedBatches { get; set; }
    public ObservableCollection<BatchGroupVM> BatchGroups { get; set; }
    
    private DateTime startDate;
    public DateTime StartDate
    {
        get => startDate;
        set
        {
            startDate = value;
            if (endDate < startDate)
            {
                endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
            OnPropertyChanged();
            _ = RefreshDataAsync();
        }
    }

    private DateTime endDate;
    public DateTime EndDate
    {
        get => endDate;
        set
        {
            endDate = value;
            startDate = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StartDate));
            _ = RefreshDataAsync();
        }
    }

    private BatchVM? selectedBatch;
    public BatchVM? SelectedBatch
    {
        get => selectedBatch;
        set
        {
            selectedBatch = value;
            OnPropertyChanged();
        }
    }

    private int unitSum;
    public int UnitSum
    {
        get => unitSum;
        set
        {
            unitSum = value;
            OnPropertyChanged();
        }
    }

    private int hitSum;
    public int HitSum
    {
        get => hitSum;
        set
        {
            hitSum = value;
            OnPropertyChanged();
        }
    }

    private int blkSum;
    public int BlkSum
    {
        get => blkSum;
        set
        {
            blkSum = value;
            OnPropertyChanged();
        }
    }

    private int pkSum;
    public int PKSum
    {
        get => pkSum;
        set
        {
            pkSum = value;
            OnPropertyChanged();
        }
    }

    private int spSum;
    public int SPSum
    {
        get => spSum;
        set
        {
            spSum = value;
            OnPropertyChanged();
        }
    }

    /* Filters */

    private bool showWeb;
    public bool ShowWeb
    {
        get => showWeb;
        set
        {
            showWeb = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private bool showMB;
    public bool ShowMB
    {
        get => showMB;
        set
        {
            showMB = value;
            OnPropertyChanged();
            ApplyFilters();
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

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public UploadBatchesCommand UploadBatchesCommand { get; set; }
    public CalculateHitsCommand CalculateHitsCommand { get; set; }
    public UploadPickLinesCommand UploadPickLinesCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public MultiAddTagCommand MultiAddTagCommand { get; set; }
    public MultiRemoveTagCommand MultiRemoveTagCommand { get; set; }

    #endregion

    private MainBatchVM(Helios helios)
    {
        Helios = helios;

        AllBatches = new List<BatchVM>();

        Batches = new ObservableCollection<BatchVM>();
        SelectedBatches = new ObservableCollection<BatchVM>();
        BatchGroups = new ObservableCollection<BatchGroupVM>();

        filterString = string.Empty;
        startDate = DateTime.Today;
        endDate = DateTime.Today;

        RefreshDataCommand = new RefreshDataCommand(this);
        UploadBatchesCommand = new UploadBatchesCommand(this);
        CalculateHitsCommand = new CalculateHitsCommand(this);
        UploadPickLinesCommand = new UploadPickLinesCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        MultiAddTagCommand = new MultiAddTagCommand(this);
        MultiRemoveTagCommand = new MultiRemoveTagCommand(this);
    }

    private async Task<MainBatchVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<MainBatchVM> CreateAsync(Helios helios)
    {
        var ret = new MainBatchVM(helios);
        return ret.InitializeAsync();
    }

    public static MainBatchVM CreateEmpty(Helios helios) => new(helios);

    public async Task RefreshDataAsync()
    {
        var currentSelected = SelectedBatch;
        Batches.Clear();
        BatchGroups.Clear();

        AllBatches = (await Helios.InventoryReader.BatchesAsync(b =>
            b.Persist || (b.CreatedOn >= StartDate && b.CreatedOn <= EndDate) ||
            (b.LastTimeCartonizedDate >= StartDate && b.LastTimeCartonizedDate <= EndDate))).OrderBy(b => b.ID)
            .Select(b => new BatchVM(b, Helios))
            .ToList();

        ApplyFilters();
        SelectedBatch = Batches.FirstOrDefault(b => b.ID == currentSelected?.ID);
    }
    
    public async Task UploadBatchesAsync()
    {
        List<Batch> newBatches;
        try
        {
            newBatches = DataConversion.RawStringToBatches(General.ClipboardToString());
        }
        catch (InvalidDataException ex)
        {
            MessageBox.Show(ex.Message, "Error: Batch Data", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error converting Clipboard data to Batches.");
            throw;
        }

        await Helios.InventoryUpdater.RawBatchesAsync(newBatches);
        await RefreshDataAsync();
    }

    public void CheckSums()
    {
        HitSum = SelectedBatches.Sum(b => b.Hits);
        UnitSum = SelectedBatches.Sum(b => b.Units);
        BlkSum = SelectedBatches.Sum(b => b.BulkHits);
        PKSum = SelectedBatches.Sum(b => b.PKHits);
        SPSum = SelectedBatches.Sum(b => b.SP01Hits);
    }

    public async Task CalculateHitsAsync()
    {
        if (SelectedBatch is null) return;
        await SelectedBatch.CalculateHits();
        CheckSums();
    }

    public async Task UploadPickLinesAsync()
    {
        List<PickLine> newPickLines;
        try
        {
            newPickLines = DataConversion.RawStringToPickLines(General.ClipboardToString());
        }
        catch (InvalidDataException ex)
        {
            MessageBox.Show(ex.Message, "Error: PickLine Data", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error converting Clipboard data to PickLines.");
            throw;
        }

        await Helios.InventoryCreator.PickLinesAsync(newPickLines);
        await RefreshDataAsync();
    }

    public void ClearFilters()
    {
        showWeb = false;
        filterString = string.Empty;
        OnPropertyChanged(nameof(ShowWeb));
        OnPropertyChanged(nameof(FilterString));
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        Batches.Clear();

        var batches = AllBatches.Where(b => 
            (ShowWeb || !b.IsWeb) &&
            (ShowMB || !b.IsMakeBulk) &&
            (Regex.IsMatch(b.Description, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(b.TagString, FilterString, RegexOptions.IgnoreCase)));

        foreach (var batchVM in batches)
            Batches.Add(batchVM);
    }

    public async Task MultiAddTagAsync()
    {
        if (SelectedBatches.Count <= 1) return;

        var inputWindow = new InputWindow("Enter Tag", "New Tag");
        if (inputWindow.ShowDialog() != true) return;
        var newTag = inputWindow.InputText;
        foreach (var batchVM in SelectedBatches) batchVM.AddTag(newTag);

        await Helios.InventoryUpdater.BatchesAsync(SelectedBatches.Select(b => b.Batch).ToList());
    }

    public async Task MultiRemoveTag()
    {
        if (SelectedBatches.Count <= 1) return;

        var inputWindow = new InputWindow("Enter Tag To Remove", "Tag Delete");
        if (inputWindow.ShowDialog() != true) return;
        var tag = inputWindow.InputText;
        foreach (var batchVM in SelectedBatches) batchVM.RemoveTag(tag);

        await Helios.InventoryUpdater.BatchesAsync(SelectedBatches.Select(b => b.Batch).ToList());
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}