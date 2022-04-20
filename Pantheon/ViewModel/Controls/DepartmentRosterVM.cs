using Pantheon.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Uranus;
using Uranus.Extension;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Controls;

internal class DepartmentRosterVM : INotifyPropertyChanged
{
    public DepartmentRoster DepartmentRoster { get; set; }
    public Helios Helios { get; set; }
    private readonly Dictionary<DateTime, DailyRosterVM> dailyRosterVMs = new();
    public readonly Dictionary<int, EmployeeRosterVM> EmployeeRosterVMs = new();

    public Dictionary<string, ShiftCounter> TargetAccessDict { get; set; }

    #region INotifyPropertyChanged Members 

    public ObservableCollection<EmployeeRosterVM> EmployeeRosters { get; set; }

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

    private ObservableCollection<ShiftCounter> shiftTargets;
    public ObservableCollection<ShiftCounter> ShiftTargets
    {
        get => shiftTargets;
        set
        {
            shiftTargets = value;
            OnPropertyChanged();
        }
    }

    #endregion

    /*/// <summary>
    /// For Testing purposes.
    /// </summary>
    public DepartmentRosterVM()
    {
        DepartmentRoster = new DepartmentRoster();
        Helios = new Helios(Settings.Default.SolLocation);
        EmployeeRosters = new ObservableCollection<EmployeeRosterVM>();
        IsInitialized = true;
        shifts = new ObservableCollection<Shift>();
        shiftTargets = new ObservableCollection<KeyValuePair<Shift, int>>();
    }*/

    public DepartmentRosterVM(DepartmentRoster roster, Helios helios)
    {
        DepartmentRoster = roster;
        Helios = helios;
        EmployeeRosters = new ObservableCollection<EmployeeRosterVM>();
        IsInitialized = false;
        shifts = new ObservableCollection<Shift>();
        shiftTargets = new ObservableCollection<ShiftCounter>();
        TargetAccessDict = new Dictionary<string, ShiftCounter>();
    }

    /// <summary>
    /// Full initialization can take some time, so only call to initialize when the specific Department Roster is to be used/viewed.
    /// </summary>
    public void Initialize()
    {
        if (IsInitialized) return;

        if (!DepartmentRoster.IsLoaded) Helios.StaffReader.FillDepartmentRoster(DepartmentRoster);

        // Shift Targets
        foreach (var (_, shift) in DepartmentRoster.ShiftDict)
        {
            Shifts.Add(shift);
            var counter = new ShiftCounter(shift, shift.DailyTarget);
            ShiftTargets.Add(counter);
            TargetAccessDict.Add(shift.ID, counter);
        }

        // Daily rosters.
        foreach (var dailyRoster in DepartmentRoster.DailyRosters)
        {
            var drVM = new DailyRosterVM(dailyRoster, this);
            dailyRosterVMs.Add(dailyRoster.Date, drVM);
            switch (dailyRoster.Day)
            {
                case DayOfWeek.Monday:
                    MondayRoster = drVM;
                    break;
                case DayOfWeek.Tuesday:
                    TuesdayRoster = drVM;
                    break;
                case DayOfWeek.Wednesday:
                    WednesdayRoster = drVM;
                    break;
                case DayOfWeek.Thursday:
                    ThursdayRoster = drVM;
                    break;
                case DayOfWeek.Friday:
                    FridayRoster = drVM;
                    break;
                case DayOfWeek.Saturday:
                    SaturdayRoster = drVM;
                    break;
                case DayOfWeek.Sunday:
                    SundayRoster = drVM;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dailyRoster.Day), dailyRoster.Day, "Unaccounted day of the week.");
            }
        }

        // EmployeeRosters
        foreach (var employeeRoster in DepartmentRoster.EmployeeRosters)
        {
            var erVM = new EmployeeRosterVM(employeeRoster, this);
            EmployeeRosterVMs.Add(employeeRoster.EmployeeID, erVM);
            foreach (var roster in employeeRoster.Rosters)
            {
                dailyRosterVMs.TryGetValue(roster.Date, out var drVM);
                if (drVM is null)
                {
                    drVM = new DailyRosterVM(new DailyRoster(DepartmentRoster.Department!, DepartmentRoster, roster.Date), this);
                    dailyRosterVMs.Add(roster.Date, drVM);
                }
                erVM.AddRoster(roster, drVM);
            }
            EmployeeRosters.Add(erVM);
        }

        IsInitialized = true;
    }

    public void AddCount(Shift shift)
    {
        TargetAccessDict[shift.ID].Count++;
    }

    public void SubCount(Shift shift)
    {
        TargetAccessDict[shift.ID].Count--;
    }

    /// <summary>
    /// Use to automate shift assignment.
    /// </summary>
    public void GenerateRosterAssignments()
    {
        AssignDefaults();
        CountToTargets();
        ApplyDepartmentDefault();
    }

    /// <summary>
    /// For a fresh start, un-assign all shifts.
    /// </summary>
    public void UnAssignAll()
    {
        foreach (var employeeRosterVM in EmployeeRosters) employeeRosterVM.SelectedShift = null;
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
        if (DepartmentRoster.Department is null) throw new DataException("Department Roster has null value for department.");

        // Get every non-default shift with a target above 0.
        var targetShifts = DepartmentRoster.Department.Shifts.Where(s => !s.Default && s.DailyTarget > 0)
            .ToDictionary(s => s, _ => new List<EmployeeRosterVM>());

        foreach (var (shift, _) in targetShifts)
            targetShifts[shift] = EmployeeRosters.Where(er => er.Employee.Shifts.Contains(shift)).ToList();

        // Order by most needed (discrepancy between those available and number required to reach target.
        targetShifts = targetShifts.OrderBy(s => s.Value.Count - TargetAccessDict[s.Key.ID].Discrepancy)
            .ToDictionary(e => e.Key, e => e.Value);

        foreach (var (shift, empRosters) in targetShifts)
        {
            if (TargetAccessDict[shift.ID].Discrepancy <= 0) continue;
            // Randomize employees #TODO: Check against history to rotate through staff (instead of randomizing).
            empRosters.Shuffle();

            foreach (var employeeRoster in empRosters.Where(employeeRoster => employeeRoster.SelectedShift is null).TakeWhile(_ => TargetAccessDict[shift.ID].Lacking))
                employeeRoster.SelectedShift = shift;
        }
    }

    /// <summary>
    /// Checks all employees to see if they are assigned. If they aren't, use department defaults if they exist,
    /// otherwise assign a shift that they are eligible for that is closest to its target count.
    /// </summary>
    public void ApplyDepartmentDefault()
    {
        foreach (var employeeRoster in EmployeeRosters.Where(employeeRoster => employeeRoster.SelectedShift is null))
        {
            if (DepartmentRoster.DefaultShift is not null)
            {
                employeeRoster.SelectedShift = DepartmentRoster.DefaultShift;
                continue;
            }

            if (employeeRoster.Employee is null)
                throw new DataException("Employee roster does not have employee initialized.");

            var bestShift = ShiftTargets.First(c => c.Discrepancy == ShiftTargets.Min(counter => counter.Discrepancy)).Shift;

            employeeRoster.SelectedShift = bestShift;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [Annotations.NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString() => DepartmentRoster.ToString();
}