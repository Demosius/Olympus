using Pantheon.ViewModels.Commands.Rosters;
using Pantheon.ViewModels.Commands.Shifts;
using Pantheon.Views.PopUp.Rosters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using Pantheon.Annotations;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Rosters;

public enum ERosterSortOption
{
    Name,
    Shift,
    ID
}

public class DepartmentRosterVM : INotifyPropertyChanged, IFilters
{
    public DepartmentRoster DepartmentRoster { get; set; }
    public Helios Helios { get; set; }

    public readonly Dictionary<int, EmployeeRosterVM> EmployeeRosterVMs = new();

    public Dictionary<string, WeeklyShiftCounter> TargetAccessDict { get; set; }

    public bool Archived { get; set; }

    #region INotifyPropertyChanged Members

    private ObservableCollection<EmployeeRosterVM> displayRosters;
    public ObservableCollection<EmployeeRosterVM> DisplayRosters
    {
        get => displayRosters;
        set
        {
            displayRosters = value;
            OnPropertyChanged();
        }
    }

    private bool isInitialized;
    public bool IsInitialized
    {
        get => isInitialized;
        set
        {
            isInitialized = value;
            OnPropertyChanged();
        }
    }

    private DailyRosterVM? mondayRoster;
    public DailyRosterVM? MondayRoster
    {
        get => mondayRoster;
        set
        {
            mondayRoster = value;
            OnPropertyChanged();
        }
    }

    private DailyRosterVM? tuesdayRoster;
    public DailyRosterVM? TuesdayRoster
    {
        get => tuesdayRoster;
        set
        {
            tuesdayRoster = value;
            OnPropertyChanged();
        }
    }

    private DailyRosterVM? wednesdayRoster;
    public DailyRosterVM? WednesdayRoster
    {
        get => wednesdayRoster;
        set
        {
            wednesdayRoster = value;
            OnPropertyChanged();

        }
    }

    private DailyRosterVM? thursdayRoster;
    public DailyRosterVM? ThursdayRoster
    {
        get => thursdayRoster;
        set
        {
            thursdayRoster = value;
            OnPropertyChanged();
        }
    }

    private DailyRosterVM? fridayRoster;
    public DailyRosterVM? FridayRoster
    {
        get => fridayRoster;
        set
        {
            fridayRoster = value;
            OnPropertyChanged();
        }
    }

    private DailyRosterVM? saturdayRoster;
    public DailyRosterVM? SaturdayRoster
    {
        get => saturdayRoster;
        set
        {
            saturdayRoster = value;
            OnPropertyChanged();
        }
    }

    private DailyRosterVM? sundayRoster;
    public DailyRosterVM? SundayRoster
    {
        get => sundayRoster;
        set
        {
            sundayRoster = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<Shift> shifts;
    public ObservableCollection<Shift> Shifts
    {
        get => shifts;
        set
        {
            shifts = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<WeeklyShiftCounter> shiftTargets;
    public ObservableCollection<WeeklyShiftCounter> ShiftTargets
    {
        get => shiftTargets;
        set
        {
            shiftTargets = value;
            OnPropertyChanged();
        }
    }

    private string searchString;
    public string SearchString
    {
        get => searchString;
        set
        {
            searchString = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private ERosterSortOption sortOption;
    public ERosterSortOption SortOption
    {
        get => sortOption;
        set
        {
            sortOption = value;
            OnPropertyChanged();
            ApplySorting();
        }
    }

    public bool ExceedTargets
    {
        get => DepartmentRoster.ExceedTargets;
        set
        {
            DepartmentRoster.ExceedTargets = value;
            OnPropertyChanged();
        }
    }

    private bool showTargets;
    public bool ShowTargets
    {
        get => showTargets;
        set
        {
            showTargets = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public GenerateRosterCommand GenerateRosterCommand { get; set; }
    public ClearShiftsCommand ClearShiftsCommand { get; set; }
    public LaunchPublicHolidayManagerCommand LaunchPublicHolidayManagerCommand { get; set; }

    #endregion

    public DepartmentRosterVM(DepartmentRoster roster, Helios helios)
    {
        DepartmentRoster = roster;
        Helios = helios;
        displayRosters = new ObservableCollection<EmployeeRosterVM>();
        IsInitialized = false;
        shifts = new ObservableCollection<Shift>();
        shiftTargets = new ObservableCollection<WeeklyShiftCounter>();
        TargetAccessDict = new Dictionary<string, WeeklyShiftCounter>();
        searchString = string.Empty;
        ShowTargets = true;
        Archived = roster.StartDate < DateTime.Now.Date.AddDays(-7);

        ApplySortingCommand = new ApplySortingCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        GenerateRosterCommand = new GenerateRosterCommand(this);
        ClearShiftsCommand = new ClearShiftsCommand(this);
        LaunchPublicHolidayManagerCommand = new LaunchPublicHolidayManagerCommand(this);
    }

    /// <summary>
    /// Initialise and set appropriate ViewModels from the core Models.
    /// 
    /// Full initialization can take some time, so only call to initialize when the specific Department Roster is to be used/viewed.
    /// </summary>
    public void Initialize()
    {
        if (IsInitialized) return;

        if (!DepartmentRoster.IsLoaded) Helios.StaffReader.FillDepartmentRoster(DepartmentRoster);

        // Shift Targets
        foreach (var shiftCounter in DepartmentRoster.ShiftCounters)
        {
            var shift = shiftCounter.Shift;
            if (shift is null) throw new DataException("Shift should not be null.");
            Shifts.Add(shift);
            ShiftTargets.Add(shiftCounter);
            TargetAccessDict.Add(shift.ID, shiftCounter);
        }

        // Daily rosters.
        MondayRoster = new DailyRosterVM(DepartmentRoster.MondayRoster!);
        TuesdayRoster = new DailyRosterVM(DepartmentRoster.TuesdayRoster!);
        WednesdayRoster = new DailyRosterVM(DepartmentRoster.WednesdayRoster!);
        ThursdayRoster = new DailyRosterVM(DepartmentRoster.ThursdayRoster!);
        FridayRoster = new DailyRosterVM(DepartmentRoster.FridayRoster!);
        SaturdayRoster = new DailyRosterVM(DepartmentRoster.SaturdayRoster!);
        SundayRoster = new DailyRosterVM(DepartmentRoster.SundayRoster!);

        // EmployeeRosters
        foreach (var employeeRoster in DepartmentRoster.EmployeeRosters) AddEmployeeRoster(employeeRoster);

        IsInitialized = true;

        ApplyFilters(EmployeeRosterVMs.Values);
    }

    /*
    public void AddCount(Shift shift)
    {
        TargetAccessDict[shift.ID].Count++;
    }

    public void SubCount(Shift shift)
    {
        TargetAccessDict[shift.ID].Count--;
    }
    */

    /// <summary>
    /// Check if roster is archived, and give the option to un-archive it.
    /// </summary>
    /// <returns>True if roster is archived.</returns>
    public bool ArchiveCheck()
    {
        if (!Archived) return Archived;

        var result =
            MessageBox.Show("This roster is archived and no longer in use. Are you sure you want to modify it?",
                "Roster is Archived", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes) Archived = false;

        return Archived;
    }

    public EmployeeRosterVM AddEmployeeRoster(EmployeeRoster roster)
    {
        var erVM = new EmployeeRosterVM(roster);//, this);
        EmployeeRosterVMs.Add(roster.EmployeeID, erVM);
        if (roster.Shift is not null) DepartmentRoster.AddCount(roster.Shift);
        return erVM;
    }

    /// <summary>
    /// Use to automate shift assignment.
    /// </summary>
    public void GenerateRosterAssignments()
    {
        if (ArchiveCheck()) return;
        DepartmentRoster.GenerateRosterAssignments();
    }

    /// <summary>
    /// For a fresh start, un-assign all shifts.
    /// </summary>
    public void UnAssignAll()
    {
        if (ArchiveCheck()) return;
        foreach (var employeeRosterVM in DisplayRosters) employeeRosterVM.SelectedShift = null;
    }

    public void ClearFilters()
    {
        ApplySorting(EmployeeRosterVMs.Values);
    }

    public void ApplyFilters()
    {
        List<EmployeeRosterVM> list;
        if (SearchString != "")
        {
            var regex = new Regex(SearchString, RegexOptions.IgnoreCase);
            list = EmployeeRosterVMs.Values.Where(e => regex.IsMatch(e.Employee.FullName)).ToList();
        }
        else
            list = EmployeeRosterVMs.Values.ToList();

        ApplySorting(list);
    }

    public void ApplySorting()
    {
        DisplayRosters = SortOption switch
        {
            ERosterSortOption.Name => new ObservableCollection<EmployeeRosterVM>(
                DisplayRosters.OrderBy(erVM => erVM.Employee.FullName)),
            ERosterSortOption.Shift => new ObservableCollection<EmployeeRosterVM>(
                DisplayRosters.OrderBy(erVM => erVM.SelectedShift)),
            ERosterSortOption.ID => new ObservableCollection<EmployeeRosterVM>(
                DisplayRosters.OrderBy(erVM => erVM.Employee.ID)),
            _ => DisplayRosters
        };
    }

    public void ApplyFilters(IEnumerable<EmployeeRosterVM> list)
    {
        if (SearchString != "")
        {
            var regex = new Regex(SearchString, RegexOptions.IgnoreCase);
            list = list.Where(e => regex.IsMatch(e.Employee.FullName)).ToList();
        }

        ApplySorting(list);
    }

    public void ApplySorting(IEnumerable<EmployeeRosterVM> list)
    {
        DisplayRosters = SortOption switch
        {
            ERosterSortOption.Name => new ObservableCollection<EmployeeRosterVM>(
                list.OrderBy(erVM => erVM.Employee.FullName)),
            ERosterSortOption.Shift => new ObservableCollection<EmployeeRosterVM>(
                list.OrderBy(erVM => erVM.SelectedShift)),
            ERosterSortOption.ID => new ObservableCollection<EmployeeRosterVM>(
                list.OrderBy(erVM => erVM.Employee.ID)),
            _ => DisplayRosters
        };
    }

    public void LaunchPublicHolidayManager()
    {
        var publicHolidayWindow = new PublicHolidayWindow(this);

        publicHolidayWindow.ShowDialog();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString() => DepartmentRoster.ToString();
}