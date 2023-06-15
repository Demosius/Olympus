using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Argos.Interfaces;
using Argos.Properties;
using Argos.ViewModels.Commands;
using Argos.Views.PopUps;
using Microsoft.Win32;
using Morpheus.Views.Windows;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;
using Path = System.IO.Path;

namespace Argos.ViewModels.Components;

public class CCNCommandVM : INotifyPropertyChanged, IFilters, IBatchTOGroupHandler
{
    public Helios Helios { get; set; }
    public ProcessedBatchDataVM ProcessedDataVM { get; set; }

    public List<BatchTOGroupVM> AllGroups { get; set; }

    #region INotifyPropertChanged Members

    public ObservableCollection<BatchTOGroupVM> Groups { get; set; }
    public ObservableCollection<BatchTOGroupVM> SelectedGroups { get; set; }

    public ObservableCollection<BatchTOLineVM>? SelectedLines => SelectedGroup?.ObservableLines;

    private BatchTOGroupVM? selectedGroup;
    public BatchTOGroupVM? SelectedGroup
    {
        get => selectedGroup;
        set
        {
            selectedGroup = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowLines));
            OnPropertyChanged(nameof(SelectedLines));
        }
    }

    public bool AutoSplitZone
    {
        get => Settings.Default.AutoSplitZone;
        set
        {
            Settings.Default.AutoSplitZone = value;
            OnPropertyChanged();
            Settings.Default.Save();
        }
    }

    public bool AutoSplitCartons
    {
        get => Settings.Default.AutoSplitCartons;
        set
        {
            Settings.Default.AutoSplitCartons = value;
            OnPropertyChanged();
            Settings.Default.Save();
        }
    }

    public string CartonSplitString
    {
        get => Settings.Default.CartonSplitString;
        set
        {
            Settings.Default.CartonSplitString = value;
            OnPropertyChanged();
            Settings.Default.Save();
        }
    }

    public bool ShowLines => SelectedGroup is not null && SelectedGroups.Count <= 1;

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
            ApplyFilters();
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
            ApplyFilters();
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
    public GatherBatchDataCommand GatherBatchDataCommand { get; set; }
    public ProcessBatchLabelsCommand ProcessBatchLabelsCommand { get; set; }
    public ZoneSplitCommand ZoneSplitCommand { get; set; }
    public CartonSplitCommand CartonSplitCommand { get; set; }
    public BaySplitCommand BaySplitCommand { get; set; }
    public CountSplitCommand CountSplitCommand { get; set; }
    public MergeCommand MergeCommand { get; set; }
    public LaunchFileLocationMenuCommand LaunchFileLocationMenuCommand { get; set; }
    public RecoverOriginalFileCommand RecoverOriginalFileCommand { get; set; }

    #endregion

    private CCNCommandVM(Helios helios)
    {
        Helios = helios;

        ProcessedDataVM = ProcessedBatchDataVM.CreateEmptyVM(Helios);

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
        GatherBatchDataCommand = new GatherBatchDataCommand(this);
        ProcessBatchLabelsCommand = new ProcessBatchLabelsCommand(this);
        ZoneSplitCommand = new ZoneSplitCommand(this);
        CartonSplitCommand = new CartonSplitCommand(this);
        BaySplitCommand = new BaySplitCommand(this);
        CountSplitCommand = new CountSplitCommand(this);
        MergeCommand = new MergeCommand(this);
        LaunchFileLocationMenuCommand = new LaunchFileLocationMenuCommand(this);
        RecoverOriginalFileCommand = new RecoverOriginalFileCommand(this);
    }

    private async Task<CCNCommandVM> InitializeAsync()
    {
        await RefreshDataAsync();
        ProcessedDataVM = await ProcessedBatchDataVM.CreateAsync(Helios);
        return this;
    }

    public static Task<CCNCommandVM> CreateAsync(Helios helios)
    {
        var ret = new CCNCommandVM(helios);
        return ret.InitializeAsync();
    }

    public static CCNCommandVM CreateEmptyVM(Helios helios) => new(helios);

    public async Task RefreshDataAsync()
    {
        var groups = await Helios.InventoryReader.BatchTOLineDataAsync();
        AllGroups = groups.Select(group => new BatchTOGroupVM(group, Helios, this)).ToList();
        foreach (var group in AllGroups)
            await group.SetObservableLinesAsync();
        
        if (AllGroups.Any())
        {
            var lowestDate = AllGroups.Min(g => g.StartDate);
            var highestDate = AllGroups.Max(g => g.EndDate);
            if (lowestDate < StartDate)
            {
                startDate = lowestDate;
                OnPropertyChanged(nameof(StartDate));
            }

            if (highestDate > EndDate)
            {
                endDate = highestDate;
                OnPropertyChanged(nameof(EndDate));
            }
        }
        ApplyFilters();
    }

    public void ClearFilters()
    {
        startDate = DateTime.Today;
        endDate = DateTime.Today;
        zoneFilter = string.Empty;
        bayFilter = string.Empty;
        ctnSizeFilter = string.Empty;
        batchFilter = string.Empty;
        OnPropertyChanged(nameof(StartDate));
        OnPropertyChanged(nameof(EndDate));
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

    public async Task GatherBatchDataAsync()
    {
        // Select files.
        var fd = new OpenFileDialog
        {
            Multiselect = true,
            InitialDirectory = Settings.Default.BatchLoadDirectory,
            Filter = "typical files (*.xls*;*.csv)|*.xls*;*.csv|All files (*.*)|*.*",
            FilterIndex = 0
        };
        if (fd.ShowDialog() != true) return;

        // Adjust and save batch load directory if required.
        var files = fd.FileNames;
        var dir = Path.GetDirectoryName(files.First());
        Settings.Default.BatchLoadDirectory = dir;
        Settings.Default.Save();

        var lines = new List<List<BatchTOLine>>();

        // Generate all data from files.
        foreach (var filePath in files)
        {
            var newLines = await DataConversion.FileToBatchTOLinesAsync(filePath);
            if (!newLines.Any()) continue;

            lines.Add(newLines);

            // Move file to backup location.
            var fileName = Path.GetFileName(filePath);
            var destinationPath = Path.Join(Helios.BatchFileBackupDirectory, fileName);
            File.Move(filePath, destinationPath);
        }

        // Create groups to hold the data - one for each batch.
        var newGroups = lines.Select(l => new BatchTOGroup(l)).ToList();

        // Split groups based on selected options.
        if (AutoSplitZone)
        {
            var zoneGroups = newGroups.SelectMany(g => g.SplitByZone()).ToList();
            newGroups.AddRange(zoneGroups);
        }
        if (AutoSplitCartons)
        {
            var ctnGroups = newGroups.SelectMany(g => g.SplitByCartonSize(CartonSplitString.Split(',').ToList())).ToList();
            newGroups.AddRange(ctnGroups);
        }

        // Remove empty groups.
        newGroups.RemoveAll(g => !g.Lines.Any());

        // Save data to database.
        await Helios.InventoryCreator.BatchTODataAsync(newGroups);

        // Refresh data.
        await RefreshDataAsync();
    }

    public async Task ProcessBatchLabelsAsync()
    {
        if (SelectedGroup is null) return;

        await SelectedGroup.ProcessLabelsAsync();
        await RefreshDataAsync();
    }

    public async Task ZoneSplit()
    {
        SelectedGroup?.SplitByZoneAsync();

        await RefreshDataAsync();
    }

    public async Task CartonSplit()
    {
        if (SelectedGroup is null) return;
        var prompt = new InputWindow("Enter carton split:", "Carton Split");
        if (prompt.ShowDialog() != true) return;
        var split = prompt.InputText;
        await SelectedGroup.SplitByCartonsAsync(split);

        await RefreshDataAsync();
    }

    public async Task BaySplit()
    {
        if (SelectedGroup is null) return;
        var prompt = new InputWindow("Enter Start Bay Split: \n\n(e.g. 'B,F,H' or 'B-E,F-G,H-N')", "Carton Split");
        if (prompt.ShowDialog() != true) return;
        var split = prompt.InputText;
        await SelectedGroup.SplitByStartBaysAsync(split);

        await RefreshDataAsync();
    }

    public async Task CountSplit()
    {
        if (SelectedGroup is null) return;
        var prompt = new InputWindow("Enter Split Method: \n\n((Even count: n\nRatio: n:m:k\nSet numbers: n,m,k))", "Carton Split");
        if (prompt.ShowDialog() != true) return;
        var split = prompt.InputText;
        await SelectedGroup.SplitByCountAsync(split);

        await RefreshDataAsync();
    }

    public async Task Merge()
    {
        if (SelectedGroup is null) return;
        var group = SelectedGroup.Group;
        SelectedGroups.Remove(SelectedGroup);
        await SelectedGroup.MergeAsync(SelectedGroups.ToList());

        await RefreshDataAsync();
        SelectedGroup = Groups.FirstOrDefault(g => g.ID == group.ID);
    }

    public async Task RecoverOriginalFile()
    {
        if (SelectedGroup is null) return;

        await SelectedGroup.RecoverOriginalFile();

        await RefreshDataAsync();
    }

    public void CheckLines()
    {
        OnPropertyChanged(nameof(ShowLines));
    }

    public void LaunchFileLocationMenu()
    {
        var flMenu = new FileLocationMenuWindow(Helios);
        flMenu.ShowDialog();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}