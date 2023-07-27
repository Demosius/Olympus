using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using WindowsInput;
using Morpheus;
using Morpheus.ViewModels.Controls;
using Sphynx.ViewModels.Commands;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using WindowsInput.Native;
using Cursor = System.Windows.Forms.Cursor;
using MessageBox = System.Windows.MessageBox;

namespace Sphynx.ViewModels.Controls;

public enum EBinSorting
{
    Bin,
    CountDate,
    Location,
    Zone,
    Empty,
    Counted
}

public class AutoCounterVM : INotifyPropertyChanged, IDBInteraction, IFilters, ISorting
{
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public List<BinVM> AllBins { get; set; }

    public bool UseUserData { get; set; }
    public bool NoUserData => UserDataTable.Rows.Count == 0 || !UserDataTable.Columns.Contains("BinCode");

    #region INotifyPropertyChanged Members

    public ObservableCollection<BinVM> Bins { get; set; }

    private DataTable userDataTable;
    public DataTable UserDataTable
    {
        get => userDataTable;
        set
        {
            userDataTable = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(NoUserData));
        }
    }

    private BinVM? selectedBin;
    public BinVM? SelectedBin
    {
        get => selectedBin;
        set
        {
            selectedBin = value;
            OnPropertyChanged();
        }
    }

    private string locationFilter;
    public string LocationFilter
    {
        get => locationFilter;
        set
        {
            locationFilter = value;
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

    private string binFilter;
    public string BinFilter
    {
        get => binFilter;
        set
        {
            binFilter = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private DateTime fromDate;
    public DateTime FromDate
    {
        get => fromDate;
        set
        {
            fromDate = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private DateTime toDate;
    public DateTime ToDate
    {
        get => toDate;
        set
        {
            toDate = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private bool showCounted;
    public bool ShowCounted
    {
        get => showCounted;
        set
        {
            showCounted = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private bool includeNonEmpty;
    public bool IncludeNonEmpty
    {
        get => includeNonEmpty;
        set
        {
            includeNonEmpty = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private EBinSorting oldSortValue;
    public EBinSorting OldSortValue
    {
        get => oldSortValue;
        set
        {
            oldSortValue = value;
            OnPropertyChanged();
        }
    }
    private EBinSorting lastSortValue;
    public EBinSorting LastSortValue
    {
        get => lastSortValue;
        set
        {
            lastSortValue = value;
            OnPropertyChanged();
        }
    }
    private EBinSorting sortValue;
    public EBinSorting SortValue
    {
        get => sortValue;
        set
        {
            OldSortValue = lastSortValue;
            LastSortValue = sortValue;
            sortValue = value;
            OnPropertyChanged();
            ApplySorting();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public InsertUserDataCommand InsertUserDataCommand { get; set; }
    public StartCountCommand StartCountCommand { get; set; }
    public GenerateEmptyBinListCommand GenerateEmptyBinListCommand { get; set; }
    public GenerateUserBinListCommand GenerateUserBinListCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public ClearDataCommand ClearDataCommand { get; set; }
    public InvertSortCommand InvertSortCommand { get; set; }

    #endregion

    public AutoCounterVM(Helios helios, ProgressBarVM progressBar)
    {
        Helios = helios;
        ProgressBar = progressBar;

        AllBins = new List<BinVM>();
        Bins = new ObservableCollection<BinVM>();

        locationFilter = string.Empty;
        zoneFilter = string.Empty;
        binFilter = string.Empty;
        fromDate = DateTime.Now.AddYears(-1);
        toDate = DateTime.Now;
        showCounted = true;

        userDataTable = new DataTable();

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        InsertUserDataCommand = new InsertUserDataCommand(this);
        StartCountCommand = new StartCountCommand(this);
        GenerateEmptyBinListCommand = new GenerateEmptyBinListCommand(this);
        GenerateUserBinListCommand = new GenerateUserBinListCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        ClearDataCommand = new ClearDataCommand(this);
        InvertSortCommand = new InvertSortCommand(this);
    }

    public async Task InsertUserData()
    {
        ProgressBar.StartTask("Inserting User Data...");
        DataTable dt;
        try
        {
            dt = await Task.Run(() => DataConversion.RawStringToTable(General.ClipboardToString()));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to upload data: {ex.Message}", "Invalid Data", MessageBoxButton.OK, MessageBoxImage.Warning);
            ProgressBar.EndTask();
            return;
        }

        await Task.Run(() =>
        {
            DataColumn? binColumn = null;

            foreach (DataColumn dataColumn in dt.Columns)
            {
                // Check for Bin Code column.
                if (Regex.IsMatch(dataColumn.ColumnName, @"(?i)bin\s?code"))
                {
                    binColumn = dataColumn;
                    break;
                }
                // Check for Code Column
                if (Regex.IsMatch(dataColumn.ColumnName, @"(?i)code")) binColumn = dataColumn;
            }

            if (binColumn is null)
            {
                MessageBox.Show(@"No valid data found on clipboard.", @"No Data", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                ProgressBar.EndTask();
                return;
            }

            binColumn.ColumnName = "BinCode";

            // Establish a counted column to track counting progress
            var countColumn = new DataColumn("Counted", typeof(bool));
            dt.Columns.Add(countColumn);
            var i = dt.Columns.IndexOf(countColumn);
            foreach (DataRow dataRow in dt.Rows)
                dataRow[i] = false;

            countColumn.SetOrdinal(0);
            binColumn.SetOrdinal(1);

            UseUserData = true;

        });

        UserDataTable = dt;

        ProgressBar.EndTask();
    }

    public async Task GenerateEmptyBinList()
    {
        UseUserData = false;
        await RefreshDataAsync();
    }

    public async Task GenerateUserBinList()
    {
        UseUserData = true;
        if (NoUserData)
        {
            MessageBox.Show(@"There is no valid user data to use.", @"No Data", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        await RefreshDataAsync();
    }

    public async Task RefreshDataAsync()
    {
        ProgressBar.StartTask("Pulling Bin Data...");
        AllBins.Clear();

        if (UseUserData && NoUserData)
        {
            ApplyFilters();
            return;
        }

        var bins = new HashSet<string>();

        if (UseUserData)
            bins = UserDataTable.AsEnumerable().Select(r => r.Field<string>("BinCode")).Where(s => s is not null)
                .ToHashSet()!;

        AllBins = (UseUserData ?
            await Helios.InventoryReader.NAVBinsAsync(b => bins.Contains(b.Code)) :
            await Helios.InventoryReader.NAVBinsAsync(b => b.Empty))
            .Select(b => new BinVM(b, Helios))
            .ToList();

        if (UseUserData)
        {
            // Check to see that all user bins are contained in bin list.
            var binCodes = AllBins.Select(b => b.Code).ToHashSet();
            var userBins = UserDataTable.AsEnumerable().Select(r => r.Field<string>("BinCode")).Where(s => s is not null).Distinct();
            var missingCodes = userBins.Where(b => !binCodes.Contains(b!));
            // Create new Bin Objects for missing bin codes.
            foreach (var missingCode in missingCodes)
                AllBins.Add(new BinVM(missingCode!, Helios));
        }

        fromDate = AllBins.Select(b => b.LastPIDate).Min();
        toDate = AllBins.Select(b => b.LastPIDate).Max();
        OnPropertyChanged(nameof(FromDate));
        OnPropertyChanged(nameof(ToDate));

        ApplyFilters();
        ProgressBar.EndTask();
    }

    public void ClearFilters()
    {
        locationFilter = string.Empty;
        zoneFilter = string.Empty;
        binFilter = string.Empty;
        fromDate = AllBins.Select(b => b.LastPIDate).Min();
        toDate = AllBins.Select(b => b.LastPIDate).Max();
        showCounted = true;
        includeNonEmpty = true;
        OnPropertyChanged(nameof(LocationFilter));
        OnPropertyChanged(nameof(ZoneFilter));
        OnPropertyChanged(nameof(BinFilter));
        OnPropertyChanged(nameof(FromDate));
        OnPropertyChanged(nameof(ToDate));
        OnPropertyChanged(nameof(ShowCounted));
        OnPropertyChanged(nameof(IncludeNonEmpty));
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        Bins.Clear();

        var bins = AllBins.Where(b =>
            Regex.IsMatch(b.Location, LocationFilter, RegexOptions.IgnoreCase) &&
            Regex.IsMatch(b.Zone, ZoneFilter, RegexOptions.IgnoreCase) &&
            Regex.IsMatch(b.Code, BinFilter, RegexOptions.IgnoreCase) &&
            b.LastPIDate >= FromDate && b.LastPIDate <= ToDate &&
            (ShowCounted || !b.Counted) &&
            (IncludeNonEmpty || b.Empty));

        bins = ApplySorting(bins);

        foreach (var binVM in bins)
            Bins.Add(binVM);
    }

    public void ApplySorting()
    {
        var bins = ApplySorting(Bins).ToList();

        Bins.Clear();
        foreach (var binVM in bins)
            Bins.Add(binVM);
    }

    public IEnumerable<BinVM> ApplySorting(IEnumerable<BinVM> bins)
    {
        // Check if all are the same.
        if (sortValue == lastSortValue && sortValue == oldSortValue)
        {
            return bins.OrderBy(b =>
                SortValue switch
                {
                    EBinSorting.Bin => b.Code,
                    EBinSorting.CountDate => b.LastPIDate.ToString("yyyyMMdd"),
                    EBinSorting.Counted => b.Counted.ToString(),
                    EBinSorting.Empty => b.Empty.ToString(),
                    EBinSorting.Location => b.Location,
                    _ => b.Zone
                });
        }

        // Check if last is the same as either the current or old.
        if (lastSortValue == sortValue)
        {
            return bins.OrderByDescending(b =>
                sortValue switch
                {
                    EBinSorting.Bin => b.Code,
                    EBinSorting.CountDate => b.LastPIDate.ToString("yyyyMMdd"),
                    EBinSorting.Counted => b.Counted.ToString(),
                    EBinSorting.Empty => b.Empty.ToString(),
                    EBinSorting.Location => b.Location,
                    _ => b.Zone
                }).ThenBy(b =>
                oldSortValue switch
                {
                    EBinSorting.Bin => b.Code,
                    EBinSorting.CountDate => b.LastPIDate.ToString("yyyyMMdd"),
                    EBinSorting.Counted => b.Counted.ToString(),
                    EBinSorting.Empty => b.Empty.ToString(),
                    EBinSorting.Location => b.Location,
                    _ => b.Zone
                });
        }

        if (lastSortValue == oldSortValue)
        {
            return bins.OrderBy(b =>
                sortValue switch
                {
                    EBinSorting.Bin => b.Code,
                    EBinSorting.CountDate => b.LastPIDate.ToString("yyyyMMdd"),
                    EBinSorting.Counted => b.Counted.ToString(),
                    EBinSorting.Empty => b.Empty.ToString(),
                    EBinSorting.Location => b.Location,
                    _ => b.Zone
                }).ThenByDescending(b =>
                oldSortValue switch
                {
                    EBinSorting.Bin => b.Code,
                    EBinSorting.CountDate => b.LastPIDate.ToString("yyyyMMdd"),
                    EBinSorting.Counted => b.Counted.ToString(),
                    EBinSorting.Empty => b.Empty.ToString(),
                    EBinSorting.Location => b.Location,
                    _ => b.Zone
                });
        }

        return bins.OrderBy(b =>
            sortValue switch
            {
                EBinSorting.Bin => b.Code,
                EBinSorting.CountDate => b.LastPIDate.ToString("yyyyMMdd"),
                EBinSorting.Counted => b.Counted.ToString(),
                EBinSorting.Empty => b.Empty.ToString(),
                EBinSorting.Location => b.Location,
                _ => b.Zone
            }).ThenBy(b =>
            lastSortValue switch
            {
                EBinSorting.Bin => b.Code,
                EBinSorting.CountDate => b.LastPIDate.ToString("yyyyMMdd"),
                EBinSorting.Counted => b.Counted.ToString(),
                EBinSorting.Empty => b.Empty.ToString(),
                EBinSorting.Location => b.Location,
                _ => b.Zone
            }).ThenBy(b =>
            oldSortValue switch
            {
                EBinSorting.Bin => b.Code,
                EBinSorting.CountDate => b.LastPIDate.ToString("yyyyMMdd"),
                EBinSorting.Counted => b.Counted.ToString(),
                EBinSorting.Empty => b.Empty.ToString(),
                EBinSorting.Location => b.Location,
                _ => b.Zone
            });
    }

    public void InvertSort()
    {
        SortValue = SortValue;
    }

    public void ClearData()
    {
        Bins.Clear();
        AllBins.Clear();
        UserDataTable.Clear();
    }

    public async Task StartCount()
    {
        var inputSimulator = new InputSimulator();

        if (MessageBox.Show(@"Move your mouse to the center of the 'Confirm' button, and press enter.", "Confirm Position",
                MessageBoxButton.OKCancel, MessageBoxImage.Information) != MessageBoxResult.OK) return;
        var confirmPos = MouseScreenPosition();

        //var errorPos = new System.Drawing.Point(confirmPos.X - 112, confirmPos.Y + 173);

        if (MessageBox.Show(@"Move your mouse to the center of the 'Empty' button, and press enter.", "Empty Position",
                MessageBoxButton.OKCancel, MessageBoxImage.Information) != MessageBoxResult.OK) return;
        Console.ReadLine();
        var emptyPos = MouseScreenPosition();

        if (MessageBox.Show(@"Move your mouse to the center of the text entry, and press enter.", "Text Position",
                MessageBoxButton.OKCancel, MessageBoxImage.Information) != MessageBoxResult.OK)
            return;
        Console.ReadLine();
        var textPos = MouseScreenPosition();

        foreach (var bin in Bins)
        {
            SelectedBin = bin;
            await bin.Count();
            if (!NoUserData)
                foreach (var row in UserDataTable.AsEnumerable().Where(row => row.Field<string>("BinCode") == bin.Code))
                    row["Counted"] = true;

            inputSimulator.Mouse.MoveMouseTo(textPos.x, textPos.y);
            inputSimulator.Mouse.LeftButtonClick();
            // Optionally add a small delay here if needed
            System.Threading.Thread.Sleep(100);
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.HOME);
            for (var i = 0; i < 15; i++) inputSimulator.Keyboard.KeyPress(VirtualKeyCode.DELETE);
            inputSimulator.Keyboard.TextEntry(bin.Code);
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            System.Threading.Thread.Sleep(100);
            //inputSimulator.Mouse.MoveMouseToPositionOnVirtualDesktop(errorPos.X, errorPos.Y);
            inputSimulator.Mouse.MoveMouseTo(emptyPos.x, emptyPos.y);
            inputSimulator.Mouse.LeftButtonClick();
            System.Threading.Thread.Sleep(100);
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            System.Threading.Thread.Sleep(100);
            inputSimulator.Mouse.MoveMouseTo(confirmPos.x, confirmPos.y);
            inputSimulator.Mouse.LeftButtonClick();
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            // Optionally add a small delay between actions if needed
            System.Threading.Thread.Sleep(100);
        }
    }

    private static (double x, double y) MouseScreenPosition()
    {
        var screenWidth = (double)Screen.PrimaryScreen.Bounds.Width;
        var screenHeight = (double)Screen.PrimaryScreen.Bounds.Height;
        (double width, double height) screenBounds = (screenWidth, screenHeight);

        return MouseScreenPosition(Cursor.Position, screenBounds);
    }

    private static (double x, double y) MouseScreenPosition(System.Drawing.Point point,
        (double width, double height) screenBounds) =>
        (point.X * 65535 / screenBounds.width, point.Y * 65535 / screenBounds.height);

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}