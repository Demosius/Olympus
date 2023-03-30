using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Pantheon.Annotations;
using Pantheon.ViewModels.Commands.Rosters;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Rosters;

public class EmployeeRosterVM : INotifyPropertyChanged
{
    public DepartmentRosterVM DepartmentRosterVM { get; set; }
    public EmployeeRoster EmployeeRoster { get; set; }

    public string EmployeeName => Employee.FullName;

    public string EmployeeIcon => Employee.Icon?.FullPath ?? string.Empty;

    public bool IsInactive => !Employee.IsActive || Employee.EmploymentType == EEmploymentType.SA;

    #region DepartmentRoster Access

    public Department? Department => DepartmentRosterVM.Department;

    public bool UseSaturdays => DepartmentRosterVM.UseSaturdays;
    public bool UseSundays => DepartmentRosterVM.UseSundays;
    public DateTime StartDate => DepartmentRosterVM.StartDate;

    public bool IsPublicHoliday(DateTime date) => DepartmentRosterVM.IsPublicHoliday(date);
    public void PromptPublicHoliday(DateTime date) => DepartmentRosterVM.PromptPublicHoliday(date);

    public int ActiveDays => RosterVMs().Count(r => r.AtWork);

    public int DaysInUse => DepartmentRosterVM.DaysInUse;

    #endregion

    #region Assignment Targets

    public List<ShiftRule> ShiftRules { get; set; }
    public IEnumerable<ShiftRuleRoster> RosterRules => ShiftRules.OfType<ShiftRuleRoster>();
    public IEnumerable<ShiftRuleRecurring> RecurringRules => ShiftRules.OfType<ShiftRuleRecurring>();
    public IEnumerable<ShiftRuleSingle> SingleRules => ShiftRules.OfType<ShiftRuleSingle>();

    public Shift? RuleShift { get; set; }
    public int MinDays { get; set; }
    public int MaxDays { get; set; }
    public List<Shift> ShiftOptions { get; set; }

    public bool TargetsSet { get; set; }

    #endregion

    #region INotifyPropertyChanged Members

    private RosterVM? mondayRoster;
    public RosterVM? MondayRoster
    {
        get => mondayRoster;
        set
        {
            mondayRoster = value;
            OnPropertyChanged();
        }
    }

    private RosterVM? tuesdayRoster;
    public RosterVM? TuesdayRoster
    {
        get => tuesdayRoster;
        set
        {
            tuesdayRoster = value;
            OnPropertyChanged();
        }
    }

    private RosterVM? wednesdayRoster;
    public RosterVM? WednesdayRoster
    {
        get => wednesdayRoster;
        set
        {
            wednesdayRoster = value;
            OnPropertyChanged();
        }
    }

    private RosterVM? thursdayRoster;
    public RosterVM? ThursdayRoster
    {
        get => thursdayRoster;
        set
        {
            thursdayRoster = value;
            OnPropertyChanged();
        }
    }

    private RosterVM? fridayRoster;
    public RosterVM? FridayRoster
    {
        get => fridayRoster;
        set
        {
            fridayRoster = value;
            OnPropertyChanged();
        }
    }

    private RosterVM? saturdayRoster;
    public RosterVM? SaturdayRoster
    {
        get => saturdayRoster;
        set
        {
            saturdayRoster = value;
            OnPropertyChanged();
        }
    }

    private RosterVM? sundayRoster;
    public RosterVM? SundayRoster
    {
        get => sundayRoster;
        set
        {
            sundayRoster = value;
            OnPropertyChanged();
        }
    }

    private Employee employee;
    public Employee Employee
    {
        get => employee;
        set
        {
            employee = value;
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

    public Shift? SelectedShift
    {
        get => EmployeeRoster.Shift;
        set
        {
            if (SelectedShift is not null && SelectedRosterType == ERosterType.Standard) SubWeeklyCount(SelectedShift);
            EmployeeRoster.Shift = value;
            EmployeeRoster.ShiftID = value?.ID ?? "";
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShiftName));
            if (SelectedShift is not null && SelectedRosterType == ERosterType.Standard) AddWeeklyCount(SelectedShift);
            SetShift(value);
        }
    }

    public string ShiftName => SelectedShift?.Name ?? "";

    public ERosterType SelectedRosterType
    {
        get => EmployeeRoster.RosterType;
        set
        {
            var adjustCounter = value != SelectedRosterType;
            EmployeeRoster.RosterType = value;
            OnPropertyChanged();
            SetRosterType(value);
            if (!adjustCounter || SelectedShift is null) return;
            if (SelectedRosterType == ERosterType.Standard)
                AddWeeklyCount(SelectedShift);
            else
                SubWeeklyCount(SelectedShift);
        }
    }

    #endregion

    #region Commands

    public DeleteEmployeeRosterCommand DeleteEmployeeRosterCommand { get; set; }

    #endregion

    public EmployeeRosterVM(EmployeeRoster employeeRoster, DepartmentRosterVM departmentRosterVM)
    {
        if (employeeRoster.Employee is null) throw new DataException("Employee Roster missing Employee Value.");
        if (departmentRosterVM.Department is null) throw new DataException("Department Roster missing Department Value.");

        EmployeeRoster = employeeRoster;
        employee = EmployeeRoster.Employee;
        DepartmentRosterVM = departmentRosterVM;

        MondayRoster = new RosterVM(employeeRoster.GetDaily(DayOfWeek.Monday), this);
        TuesdayRoster = new RosterVM(employeeRoster.GetDaily(DayOfWeek.Tuesday), this);
        WednesdayRoster = new RosterVM(employeeRoster.GetDaily(DayOfWeek.Wednesday), this);
        ThursdayRoster = new RosterVM(employeeRoster.GetDaily(DayOfWeek.Thursday), this);
        FridayRoster = new RosterVM(employeeRoster.GetDaily(DayOfWeek.Friday), this);
        SaturdayRoster = new RosterVM(employeeRoster.GetDaily(DayOfWeek.Saturday), this);
        SundayRoster = new RosterVM(employeeRoster.GetDaily(DayOfWeek.Sunday), this);

        shifts = new ObservableCollection<Shift>(employeeRoster.Shifts);

        DeleteEmployeeRosterCommand = new DeleteEmployeeRosterCommand(this);

        ShiftOptions = new List<Shift>();
        ShiftRules = new List<ShiftRule>();
    }

    public void SubDailyCount(Shift shift, DayOfWeek day) => DepartmentRosterVM.SubDailyCount(shift, day);

    public void AddDailyCount(Shift shift, DayOfWeek day) => DepartmentRosterVM.AddDailyCount(shift, day);

    public void SubWeeklyCount(Shift shift) => DepartmentRosterVM.SubWeeklyCount(shift);

    public void AddWeeklyCount(Shift shift) => DepartmentRosterVM.AddWeeklyCount(shift);

    /// <summary>
    /// OnPropertyChanged notify for all rosters.
    /// </summary>
    public void NotifyDailies()
    {
        OnPropertyChanged(nameof(SelectedShift));
        OnPropertyChanged(nameof(MondayRoster));
        OnPropertyChanged(nameof(TuesdayRoster));
        OnPropertyChanged(nameof(WednesdayRoster));
        OnPropertyChanged(nameof(ThursdayRoster));
        OnPropertyChanged(nameof(FridayRoster));
        OnPropertyChanged(nameof(SaturdayRoster));
        OnPropertyChanged(nameof(SundayRoster));
    }

    public void SetShiftAssignmentTargets(bool force = false)
    {
        if (TargetsSet && !force) return;

        // Gather rules that apply to this weekly roster, from the employee.
        ShiftRules = Employee.ShiftRules.Where(rule => rule.AppliesToWeek(StartDate)).ToList();
        RuleShift = RuleBasedShift();
        ShiftOptions = Employee.Shifts;

        if (RuleShift is not null && !ShiftOptions.Contains(RuleShift)) ShiftOptions.Add(RuleShift);

        (MinDays, MaxDays) = GetShiftMinMax();

        TargetsSet = true;
    }

    /// <summary>
    /// Uses the rules to determine the minimum and maximum number of days this employee can work this week.
    /// </summary>
    /// <returns>(min, max)</returns>
    private (int, int) GetShiftMinMax()
    {
        bool? monday = null;
        bool? tuesday = null;
        bool? wednesday = null;
        bool? thursday = null;
        bool? friday = null;
        bool? saturday = null;
        bool? sunday = null;

        var minDays = 0;
        var maxDays = 7;

        foreach (var rosterRule in RosterRules)
        {
            monday = CalculateAttendance(monday, rosterRule.Monday);
            tuesday = CalculateAttendance(tuesday, rosterRule.Tuesday);
            wednesday = CalculateAttendance(wednesday, rosterRule.Wednesday);
            thursday = CalculateAttendance(thursday, rosterRule.Thursday);
            friday = CalculateAttendance(friday, rosterRule.Friday);
            saturday = CalculateAttendance(saturday, rosterRule.Saturday);
            sunday = CalculateAttendance(sunday, rosterRule.Sunday);

            minDays = rosterRule.MinDays > minDays ? rosterRule.MinDays : minDays;
            maxDays = rosterRule.MaxDays < maxDays ? rosterRule.MaxDays : maxDays;
        }

        var min = (monday == true ? 1 : 0) + (tuesday == true ? 1 : 0) +
                  (wednesday == true ? 1 : 0) + (thursday == true ? 1 : 0) +
                  (friday == true ? 1 : 0) + (saturday == true ? 1 : 0) + (sunday == true ? 1 : 0);
        var max = 7 - (monday == false ? 1 : 0) - (tuesday == false ? 1 : 0) -
                  (wednesday == false ? 1 : 0) - (thursday == false ? 1 : 0) -
                  (friday == false ? 1 : 0) - (saturday == false ? 1 : 0) - (sunday == false ? 1 : 0);

        min = minDays > min ? minDays : min;
        max = maxDays < max ? maxDays : max;

        if (max > DaysInUse) max = DaysInUse;
        if (min > max) min = max;

        return (min, max);
    }

    private static bool? CalculateAttendance(bool? first, bool? second)
        => first == false || second == false ? false : first == true || second == true ? true : null;


    /// <summary>
    /// Determine required shift based on rules.
    /// </summary>
    /// <returns>Null if there is no set required shift.</returns>
    private Shift? RuleBasedShift()
    {
        var shiftDedication = new Dictionary<Shift, int>();

        foreach (var rule in ShiftRules)
        {
            var dedication = rule.ShiftDedication();
            if (dedication is null) continue;
            var shift = dedication.Value.Item1;
            var newCount = dedication.Value.Item2;

            if (!shiftDedication.ContainsKey(shift))
                shiftDedication.Add(shift, newCount);
            else
                shiftDedication[dedication.Value.Item1] += newCount;
        }

        if (shiftDedication.Count == 0) return Employee.DefaultShift;
        var maxDedication = shiftDedication.Values.Max();

        return shiftDedication.First(kv => kv.Value == maxDedication).Key;
    }
    
    /// <summary>
    /// Sets teh shift to the employee's default if they have one.
    /// </summary>
    public void SetDefault()
    {
        var shift = Employee.DefaultShift;
        if (shift is null) return;

        SelectedShift = shift;
    }

    public void SetPublicHoliday(DayOfWeek dayOfWeek, bool isPublicHoliday) => Roster(dayOfWeek)?.SetPublicHoliday(isPublicHoliday);

    public RosterVM? Roster(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Sunday => SundayRoster,
            DayOfWeek.Monday => MondayRoster,
            DayOfWeek.Tuesday => TuesdayRoster,
            DayOfWeek.Wednesday => WednesdayRoster,
            DayOfWeek.Thursday => ThursdayRoster,
            DayOfWeek.Friday => FridayRoster,
            DayOfWeek.Saturday => SaturdayRoster,
            _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
        };
    }

    public void SetRosterType(ERosterType type)
    {
        foreach (var rosterVM in RosterVMs())
            rosterVM.Type = rosterVM.IsPublicHoliday ? ERosterType.PublicHoliday : type;
    }

    public void SetShift(Shift? shift)
    {
        foreach (var rosterVM in RosterVMs())
            rosterVM.SelectedShift = shift;
    }

    public void CalculateShift(Dictionary<Shift, float> daysPerWeekPerShift)
    {
        if (RuleShift is not null)
        {
            SelectedShift = RuleShift;
            return;
        }

        // Start with shifts that are not over target.
        var shiftChoice = ShiftOptions.Where(shift => !DepartmentRosterVM.ShiftCounter(shift).OverTarget || (shift.Default && DepartmentRosterVM.ExceedTargets)).ToList();

        if (!shiftChoice.Any())
        {
            SelectedShift = null;
            SelectedRosterType = ERosterType.RDO;
            return;
        }

        // Remove shifts below minimum.
        for (var i = shiftChoice.Count - 1; i >= 0; i--)
        {
            if (shiftChoice.Count <= 1) break;
            var shift = shiftChoice[i];
            if (!daysPerWeekPerShift.TryGetValue(shift, out var targetDays)) continue;
            if (targetDays < MinDays) shiftChoice.RemoveAt(i);
        }

        // Remove shifts above maximum.
        for (var i = shiftChoice.Count - 1; i >= 0; i--)
        {
            if (shiftChoice.Count <= 1) break;
            var shift = shiftChoice[i];
            if (!daysPerWeekPerShift.TryGetValue(shift, out var targetDays)) continue;
            if (targetDays > MaxDays) shiftChoice.RemoveAt(i);
        }

        // Get remaining shifts ordered in a particular way? 
        var shiftPriority = DepartmentRosterVM.CalculateShiftPriority(shiftChoice).ToList();

        if (shiftPriority.Any())
        {
            SelectedShift = shiftPriority.First().Item1;
            return;
        }

        if (MinDays == 0)
        {
            SelectedShift = null;
            SelectedRosterType = ERosterType.RDO;
            return;
        }

        var def = Employee.DefaultShift;
        def ??= shiftChoice.FirstOrDefault(s => s.Default);
        def ??= shiftChoice.FirstOrDefault();
        SelectedShift = def;
    }

    /// <summary>
    /// Adjust daily rosters to bring numbers closer to the targets for daily shift counters.
    /// </summary>
    /// <param name="daysPerWeekPerShift"></param>
    /// <returns></returns>
    public int AdjustDailyShifts(Dictionary<Shift, float> daysPerWeekPerShift)
    {
        if (SelectedShift is null) return 0;

        // If there is a shift, assume that employee must get at least one shift.
        MinDays = MinDays == 0 ? 1 : MinDays;

        var rand = new Random();
        var randomizedRosters = RosterVMs().OrderBy(_ => rand.Next()).ToList();

        foreach (var roster in randomizedRosters) roster.ApplyShiftRules(ShiftRules);

        if (!daysPerWeekPerShift.TryGetValue(SelectedShift, out var targetDaysPerWeek))
            targetDaysPerWeek = DepartmentRosterVM.GetAverageDaysPerWeek(SelectedShift);

        if (MinDays > targetDaysPerWeek) targetDaysPerWeek = MinDays;
        if (MaxDays < targetDaysPerWeek) targetDaysPerWeek = MaxDays;

        var days = ActiveDays;

        var targetShift = targetDaysPerWeek - days;

        var adjustments = 0;

        foreach (var roster in randomizedRosters)
        {
            if (targetShift == 0) break;
            var shifted = roster.AdjustShift(targetShift);
            adjustments += Math.Abs(shifted);
            targetShift -= shifted;
        }


        return adjustments;
    }

    private IEnumerable<RosterVM> RosterVMs()
    {
        if (Department is null) return Enumerable.Empty<RosterVM>();

        var returnList = new List<RosterVM>
        {
            (MondayRoster ??= new RosterVM(EmployeeRoster.GetDaily(DayOfWeek.Monday), this)),
            (TuesdayRoster ??= new RosterVM(EmployeeRoster.GetDaily(DayOfWeek.Tuesday), this)),
            (WednesdayRoster ??= new RosterVM(EmployeeRoster.GetDaily(DayOfWeek.Wednesday), this)),
            (ThursdayRoster ??= new RosterVM(EmployeeRoster.GetDaily(DayOfWeek.Thursday), this)),
            (FridayRoster ??= new RosterVM(EmployeeRoster.GetDaily(DayOfWeek.Friday), this))
        };
        if (UseSaturdays) returnList.Add(SaturdayRoster ??= new RosterVM(EmployeeRoster.GetDaily(DayOfWeek.Saturday), this));
        if (UseSundays) returnList.Add(SundayRoster ??= new RosterVM(EmployeeRoster.GetDaily(DayOfWeek.Sunday), this));

        return returnList;
    }

    public void Delete()
    {
        // Confirm deletion with employee.
        if (MessageBox.Show($"Are you sure that you want to delete the roster for {Employee} from the roster: {DepartmentRosterVM}?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        EmployeeRoster.Delete();
        foreach (var rosterVM in RosterVMs())
        {
            rosterVM.Delete();
        }

        DepartmentRosterVM.RemoveEmployee(this);
    }

    public DailyRosterVM? GetDailyRoster(DayOfWeek day) => DepartmentRosterVM.GetDaily(day);

    /*/// <summary>
    /// Apply shift rules from the employee to the employee (weekly) roster, and the individual rosters therein.
    /// </summary>
    public void ApplyShiftRules()
    {
        // Gather rules that apply to this weekly roster, from the employee.
        var rules = Employee.ShiftRules.Where(rule => rule.AppliesToWeek(EmployeeRoster.StartDate)).ToList();
        
        foreach (var (_, rosterVM) in rosterVMs) rosterVM.ApplyShiftRules(rules);
    }*/

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}