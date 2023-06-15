using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Argos.Interfaces;
using Argos.ViewModels.Commands;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Argos.ViewModels.Components;

public class ProcessedBatchDataVM : INotifyPropertyChanged, IFilters, IBatchTOGroupHandler
{
    public Helios Helios { get; set; }
    public List<BatchTOGroupVM> AllGroups { get; set; }

    #region INotifyPropertChanged Members

    public ObservableCollection<BatchTOGroupVM> Groups { get; set; }
    public ObservableCollection<BatchTOGroupVM> SelectedGroups { get; set; }

    private BatchTOGroupVM? selectedGroup;
    public BatchTOGroupVM? SelectedGroup
    {
        get => selectedGroup;
        set
        {
            selectedGroup = value;
            OnPropertyChanged();
        }
    }

    /******* Filters *********/

    private DateTime startDate;
    public DateTime StartDate
    {
        get => startDate;
        set
        {
            startDate = value;
            if (startDate > endDate)
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
            if (endDate < startDate)
            {
                startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
            OnPropertyChanged();
            _ = RefreshDataAsync();
        }
    }

    private string zoneFilter;
    public string ZoneFilter
    {
        get => zoneFilter;
        set
        {
            zoneFilter = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private string bayFilter;
    public string BayFilter
    {
        get => bayFilter;
        set
        {
            bayFilter = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private string ctnSizeFilter;
    public string CtnSizeFilter
    {
        get => ctnSizeFilter;
        set
        {
            ctnSizeFilter = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private string batchFilter;
    public string BatchFilter
    {
        get => batchFilter;
        set
        {
            batchFilter = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ZoneSplitCommand ZoneSplitCommand { get; set; }
    public CartonSplitCommand CartonSplitCommand { get; set; }
    public BaySplitCommand BaySplitCommand { get; set; }
    public CountSplitCommand CountSplitCommand { get; set; }
    public MergeCommand MergeCommand { get; set; }
    public RecoverOriginalFileCommand RecoverOriginalFileCommand { get; set; }

    #endregion

    private ProcessedBatchDataVM(Helios helios)
    {
        Helios = helios;

        AllGroups = new List<BatchTOGroupVM>();
        Groups = new ObservableCollection<BatchTOGroupVM>();
        SelectedGroups = new ObservableCollection<BatchTOGroupVM>();

        startDate = DateTime.Today;
        endDate = DateTime.Today;
        zoneFilter = string.Empty;
        bayFilter = string.Empty;
        ctnSizeFilter = string.Empty;
        batchFilter = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ZoneSplitCommand = new ZoneSplitCommand(this);
        CartonSplitCommand = new CartonSplitCommand(this);
        BaySplitCommand = new BaySplitCommand(this);
        CountSplitCommand = new CountSplitCommand(this);
        MergeCommand = new MergeCommand(this);
        RecoverOriginalFileCommand = new RecoverOriginalFileCommand(this);
    }

    private async Task<ProcessedBatchDataVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<ProcessedBatchDataVM> CreateAsync(Helios helios)
    {
        var ret = new ProcessedBatchDataVM(helios);
        return ret.InitializeAsync();
    }

    public static ProcessedBatchDataVM CreateEmptyVM(Helios helios) => new(helios);

    public async Task RefreshDataAsync()
    {
        // Checking against time value that may be partially through the date (and therefore larger than the raw date value.
        var checkEnd = EndDate.AddDays(1);  
        var groups = await Helios.InventoryReader.BatchTOLineDataAsync(l =>
            l.IsFinalised && 
            l.FinalProcessingTime >= StartDate &&
            l.FinalProcessingTime <= checkEnd);
        AllGroups = groups.Select(group => new BatchTOGroupVM(group, Helios, this)).ToList();
        ApplyFilters();
    }

    public void ClearFilters()
    {
        zoneFilter = string.Empty;
        bayFilter = string.Empty;
        ctnSizeFilter = string.Empty;
        batchFilter = string.Empty;
        OnPropertyChanged(nameof(ZoneFilter));
        OnPropertyChanged(nameof(BayFilter));
        OnPropertyChanged(nameof(CtnSizeFilter));
        OnPropertyChanged(nameof(BatchFilter));
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        Groups.Clear();

        var groups = AllGroups.Where(g =>
                g.EndDate >= StartDate && g.StartDate <= EndDate &&
                Regex.IsMatch(g.ZoneString, ZoneFilter, RegexOptions.IgnoreCase) &&
                Regex.IsMatch(g.StartBays, BayFilter, RegexOptions.IgnoreCase) &&
                Regex.IsMatch(g.CartonSizes, CtnSizeFilter, RegexOptions.IgnoreCase) &&
                Regex.IsMatch(g.BatchString, BatchFilter, RegexOptions.IgnoreCase))
            .OrderBy(g => g.StartDate)
            .ThenBy(g => g.BatchString)
            .ThenBy(g => g.StartBays)
            .ThenBy(g => g.CartonSizes);

        foreach (var group in groups)
            Groups.Add(group);
    }

    public Task ZoneSplit()
    {
        throw new NotImplementedException();
    }

    public Task CartonSplit()
    {
        throw new NotImplementedException();
    }

    public Task BaySplit()
    {
        throw new NotImplementedException();
    }

    public Task CountSplit()
    {
        throw new NotImplementedException();
    }

    public Task Merge()
    {
        throw new NotImplementedException();
    }

    public async Task RecoverOriginalFile()
    {
        if (SelectedGroup is null) return;

        await SelectedGroup.RecoverOriginalFile();

        await RefreshDataAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
}