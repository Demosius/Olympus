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
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.Views.PopUp.Employees;
using Styx;
using Uranus;
using Uranus.Commands;
using Uranus.Extensions;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Rosters;

public class DepartmentRosterVM : INotifyPropertyChanged, IFilters
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public DepartmentRoster DepartmentRoster { get; set; }

    public readonly List<EmployeeRosterVM> EmployeeRosters = new();
    public readonly Dictionary<int, EmployeeRosterVM> EmployeeRosterDict = new();

    public Dictionary<string, WeeklyCounterVM> TargetAccessDict { get; set; }

    public bool Archived { get; set; }

    #region Department Roster Access

    public Department? Department => DepartmentRoster.Department;

    public bool UseSaturdays => DepartmentRoster.UseSaturdays;
    public bool UseSundays => DepartmentRoster.UseSundays;
    public DateTime StartDate => DepartmentRoster.StartDate;

    public int EmployeesWithoutRosterCount => DepartmentRoster.EmployeesWithoutRoster.Count;

    public bool IsMissingRoster => EmployeesWithoutRosterCount > 0;

    public Shift? DefaultShift => DepartmentRoster.DefaultShift;

    #endregion

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

    private EmployeeRosterVM? selectedRoster;
    public EmployeeRosterVM? SelectedRoster
    {
        get => selectedRoster;
        set
        {
            selectedRoster = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowActive));
        }
    }

    public bool ShowActive => SelectedRoster is not null;

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

    private ObservableCollection<WeeklyCounterVM> shiftTargets;
    public ObservableCollection<WeeklyCounterVM> ShiftTargets
    {
        get => shiftTargets;
        set
        {
            shiftTargets = value;
            OnPropertyChanged();
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
    public GenerateRosterCommand GenerateRosterCommand { get; set; }
    public ClearShiftsCommand ClearShiftsCommand { get; set; }
    public LaunchPublicHolidayManagerCommand LaunchPublicHolidayManagerCommand { get; set; }
    public GenerateAdditionalRostersCommand GenerateAdditionalRostersCommand { get; set; }

    #endregion

    public DepartmentRosterVM(DepartmentRoster roster, Helios helios, Charon charon)
    {
        DepartmentRoster = roster;
        Helios = helios;
        Charon = charon;

        displayRosters = new ObservableCollection<EmployeeRosterVM>();
        IsInitialized = false;
        shifts = new ObservableCollection<Shift>();
        shiftTargets = new ObservableCollection<WeeklyCounterVM>();
        TargetAccessDict = new Dictionary<string, WeeklyCounterVM>();
        filterString = string.Empty;
        ShowTargets = true;
        Archived = roster.StartDate < DateTime.Now.Date.AddDays(-7);

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        GenerateRosterCommand = new GenerateRosterCommand(this);
        ClearShiftsCommand = new ClearShiftsCommand(this);
        LaunchPublicHolidayManagerCommand = new LaunchPublicHolidayManagerCommand(this);
        GenerateAdditionalRostersCommand = new GenerateAdditionalRostersCommand(this);
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

        if (Department is null) throw new DataException("Department roster initialized without Department Object.");

        // Shift Targets
        foreach (var shiftCounter in DepartmentRoster.ShiftCounters.Select(c => new WeeklyCounterVM(c)))
        {
            var shift = shiftCounter.Shift;
            if (shift is null) throw new DataException("Shift should not be null.");
            Shifts.Add(shift);
            ShiftTargets.Add(shiftCounter);
            TargetAccessDict.Add(shift.ID, shiftCounter);
        }

        // Daily rosters.
        MondayRoster = new DailyRosterVM(DepartmentRoster.GetDaily(DayOfWeek.Monday), this);
        TuesdayRoster = new DailyRosterVM(DepartmentRoster.GetDaily(DayOfWeek.Tuesday), this);
        WednesdayRoster = new DailyRosterVM(DepartmentRoster.GetDaily(DayOfWeek.Wednesday), this);
        ThursdayRoster = new DailyRosterVM(DepartmentRoster.GetDaily(DayOfWeek.Thursday), this);
        FridayRoster = new DailyRosterVM(DepartmentRoster.GetDaily(DayOfWeek.Friday), this);
        SaturdayRoster = new DailyRosterVM(DepartmentRoster.GetDaily(DayOfWeek.Saturday), this);
        SundayRoster = new DailyRosterVM(DepartmentRoster.GetDaily(DayOfWeek.Sunday), this);

        // EmployeeRosters
        foreach (var employeeRoster in DepartmentRoster.EmployeeRosters) AddEmployeeRoster(employeeRoster);

        IsInitialized = true;

        ApplyFilters();
    }

    public void AddWeeklyCount(Shift shift)
    {
        ShiftCounter(shift).Count++;
    }

    public void SubWeeklyCount(Shift shift)
    {
        ShiftCounter(shift).Count--;
    }

    public void SubDailyCount(Shift rosterShift, DayOfWeek day)
    {
        GetDaily(day)?.SubCount(rosterShift);
    }

    public void AddDailyCount(Shift rosterShift, DayOfWeek day)
    {
        GetDaily(day)?.AddCount(rosterShift);
    }

    public DailyRosterVM? GetDaily(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Sunday => SundayRoster,
            DayOfWeek.Monday => MondayRoster,
            DayOfWeek.Tuesday => TuesdayRoster,
            DayOfWeek.Wednesday => WednesdayRoster,
            DayOfWeek.Thursday => ThursdayRoster,
            DayOfWeek.Friday => FridayRoster,
            DayOfWeek.Saturday => SaturdayRoster,
            _ => throw new ArgumentOutOfRangeException(nameof(day), day, null)
        };
    }

    public WeeklyCounterVM ShiftCounter(Shift shift)
    {
        if (TargetAccessDict.TryGetValue(shift.ID, out var counter)) return counter;

        counter = new WeeklyCounterVM(DepartmentRoster.ShiftCounter(shift));
        TargetAccessDict.Add(shift.ID, counter);
        ShiftTargets.Add(counter);

        return counter;
    }
    
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
        var erVM = new EmployeeRosterVM(roster, this);
        EmployeeRosterDict.Add(roster.EmployeeID, erVM);
        EmployeeRosters.Add(erVM);
        if (roster.Shift is not null) AddWeeklyCount(roster.Shift);
        return erVM;
    }
    
    /// <summary>
    /// Use to automate shift assignment.
    /// </summary>
    public void GenerateRosterAssignments()
    {
        if (ArchiveCheck()) return;
        ApplyShiftRules();
        AssignDefaults();
        CountToTargets();
        ApplyDepartmentDefault();
        CheckPublicHolidays();
    }

    /// <summary>
    /// Applies shift rules to employees and their weekly/daily shifts as appropriate.
    /// </summary>
    private void ApplyShiftRules()
    {
        foreach (var employeeRoster in EmployeeRosters)
        {
            // TODO: 
            employeeRoster.ApplyShiftRules();
        }
    }

    /// <summary>
    /// Assigns shifts to employees based on their defined defaults - only if they do not already have assigned shifts..
    /// </summary>
    public void AssignDefaults()
    {
        foreach (var employeeRoster in EmployeeRosters.Where(employeeRoster => employeeRoster.SelectedShift is null))
            employeeRoster.SetDefault();
    }

    /// <summary>
    /// Attempt to reach the targeted number for each shift.
    /// </summary>
    public void CountToTargets()
    {
        if (Department is null) throw new DataException("Department Roster has null value for department.");

        // Get every non-default shift with a target above 0.
        var targetShifts = Department.Shifts.Where(s => !s.Default && s.DailyTarget > 0)
            .ToDictionary(s => s, _ => new List<EmployeeRosterVM>());

        foreach (var (shift, _) in targetShifts)
            targetShifts[shift] = EmployeeRosters
                .Where(er => er.Employee.Shifts.Contains(shift)).ToList();

        // Order by most needed (discrepancy between those available and number required to reach target.
        targetShifts = targetShifts.OrderBy(s => s.Value.Count - ShiftCounter(s.Key).Discrepancy)
            .ToDictionary(e => e.Key, e => e.Value);

        foreach (var (shift, empRosters) in targetShifts)
        {
            if (ShiftCounter(shift).Discrepancy <= 0) continue;
            // Randomize employees #TODO: Check against history to rotate through staff (instead of randomizing).
            empRosters.Shuffle();

            foreach (var employeeRoster in empRosters.Where(employeeRoster => employeeRoster.SelectedShift is null).TakeWhile(_ => ShiftCounter(shift).Lacking))
                employeeRoster.SelectedShift = shift;
        }
    }

    /// <summary>
    /// Checks all employees to see if they are assigned. If they aren't, use department defaults if they exist,
    /// otherwise assign a shift that they are eligible for that is closest to its target count.
    /// </summary>
    public void ApplyDepartmentDefault()
    {
        if (Shifts.Count == 0) return;

        // Ensure that all shift counters exist that are required.
        foreach (var shift in Shifts) _ = ShiftCounter(shift);

        foreach (var employeeRoster in EmployeeRosters.Where(employeeRoster => employeeRoster.SelectedShift is null))
        {
            if (DefaultShift is not null)
            {
                employeeRoster.SelectedShift = DefaultShift;
                continue;
            }

            if (employeeRoster.Employee is null)
                throw new DataException("Employee roster does not have employee initialized.");

            var bestShift = ShiftTargets.First(c => c.Discrepancy == ShiftTargets.Min(counter => counter.Discrepancy)).Shift;

            employeeRoster.SelectedShift = bestShift;
        }
    }

    /// <summary>
    /// Check against some API for public holidays.
    /// </summary>
    private void CheckPublicHolidays()
    {
        //throw new NotImplementedException();
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
        FilterString = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        List<EmployeeRosterVM> list;
        if (FilterString != "")
        {
            var regex = new Regex(FilterString, RegexOptions.IgnoreCase);
            list = EmployeeRosterDict.Values.Where(e => regex.IsMatch(e.Employee.FullName)).ToList();
        }
        else
            list = EmployeeRosterDict.Values.ToList();

        DisplayRosters.Clear();
        foreach (var employeeRosterVM in list)
            DisplayRosters.Add(employeeRosterVM);
    }

    /// <summary>
    /// Checks with the user if this date is to be set as a public holiday for all.
    /// </summary>
    public void PromptPublicHoliday(DateTime date)
    {
        if (MessageBox.Show($"Do you want to set {date:dddd, dd/MM/yyyy} as a public holiday?", "Public Holiday",
                MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
            SetPublicHoliday(date.DayOfWeek, true);
    }

    public void LaunchPublicHolidayManager()
    {
        var publicHolidayWindow = new PublicHolidayWindow(this);

        publicHolidayWindow.ShowDialog();
    }

    public void GenerateLoanRosters()
    {
        if (Department is null) throw new DataException("Department roster initialized without Department Object.");

        var employees = Helios.StaffReader.BorrowableEmployees(Department.Name)
            .Select(e => new EmployeeVM(e, Charon, Helios)).ToList();

        var employeeSelector = new EmployeeSelectionWindow(employees, false, Department.Name, true);
        if (employeeSelector.ShowDialog() != true) return;

        var rosters = DepartmentRoster.CreateMissingRosters(employeeSelector.SelectedEmployees.Select(vm => vm.Employee));

        foreach (var roster in rosters)
        {
            var rosterVM = new EmployeeRosterVM(roster, this);
            EmployeeRosterDict.Add(roster.EmployeeID, rosterVM);
            EmployeeRosters.Add(rosterVM);
        }

        ApplyFilters();
    }

    public void GenerateMissingRosters()
    {
        if (Department is null) throw new DataException("Department roster initialized without Department Object.");

        var employees = DepartmentRoster.EmployeesWithoutRoster.Select(e => new EmployeeVM(e, Charon, Helios)).ToList();

        var employeeSelector = new EmployeeSelectionWindow(employees, false, Department.Name, true);
        if (employeeSelector.ShowDialog() != true) return;

        var rosters = DepartmentRoster.CreateMissingRosters(employeeSelector.SelectedEmployees.Select(vm => vm.Employee));

        foreach (var employeeRoster in rosters)
        {
            var rosterVM = new EmployeeRosterVM(employeeRoster, this);
            EmployeeRosterDict.Add(employeeRoster.EmployeeID, rosterVM);
            EmployeeRosters.Add(rosterVM);
        }

        ApplyFilters();

        OnPropertyChanged(nameof(EmployeesWithoutRosterCount));
        OnPropertyChanged(nameof(IsMissingRoster));
    }

    public bool IsPublicHoliday(DateTime date) => GetDaily(date.DayOfWeek)?.PublicHoliday ?? false;

    public void SetPublicHoliday(DayOfWeek dayOfWeek, bool isPublicHoliday)
    {
        foreach (var employeeRosterVM in EmployeeRosters) employeeRosterVM.SetPublicHoliday(dayOfWeek, isPublicHoliday);
        var daily = GetDaily(dayOfWeek);
        if (daily is null) return;
        daily.PublicHoliday = isPublicHoliday;
    }

    /// <summary>
    /// Remove the given employee roster from all relevant lists and objects.
    /// </summary>
    /// <param name="employeeRosterVM"></param>
    public void RemoveEmployee(EmployeeRosterVM employeeRosterVM)
    {
        if (ReferenceEquals(SelectedRoster, employeeRosterVM)) SelectedRoster = null;
        DisplayRosters.Remove(employeeRosterVM);
        EmployeeRosters.Remove(employeeRosterVM);
        EmployeeRosterDict.Remove(employeeRosterVM.Employee.ID);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString() => DepartmentRoster.ToString();
}