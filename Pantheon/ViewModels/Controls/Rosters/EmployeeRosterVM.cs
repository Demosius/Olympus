using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
            if (SelectedShift is not null && SelectedRosterType == ERosterType.Standard) SubCount(SelectedShift);
            EmployeeRoster.Shift = value;
            EmployeeRoster.ShiftID = value?.ID ?? "";
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShiftName));
            if (SelectedShift is not null && SelectedRosterType == ERosterType.Standard) AddCount(SelectedShift);
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
                AddCount(SelectedShift);
            else
                SubCount(SelectedShift);
        }
    }

    #endregion

    #region Commands

    public DeleteEmployeeRosterCommand DeleteEmployeeRosterCommand { get; set; }

    #endregion

    public EmployeeRosterVM(EmployeeRoster roster, DepartmentRosterVM departmentRosterVM)
    {
        if (roster.Employee is null) throw new DataException("Employee Roster missing Employee Value.");
        if (departmentRosterVM.Department is null) throw new DataException("Department Roster missing Department Value.");

        EmployeeRoster = roster;
        employee = EmployeeRoster.Employee;
        DepartmentRosterVM = departmentRosterVM;
        
        MondayRoster = new RosterVM(roster.MondayRoster ?? new Roster(DepartmentRosterVM.Department, employee, roster.StartDate), this);
        TuesdayRoster = new RosterVM(roster.TuesdayRoster ?? new Roster(DepartmentRosterVM.Department, employee, roster.StartDate.AddDays(1)), this);
        WednesdayRoster = new RosterVM(roster.WednesdayRoster ?? new Roster(DepartmentRosterVM.Department, employee, roster.StartDate.AddDays(2)), this);
        ThursdayRoster = new RosterVM(roster.ThursdayRoster ?? new Roster(DepartmentRosterVM.Department, employee, roster.StartDate.AddDays(3)), this);
        FridayRoster = new RosterVM(roster.FridayRoster ?? new Roster(DepartmentRosterVM.Department, employee, roster.StartDate.AddDays(4)), this);
        SaturdayRoster = new RosterVM(roster.SaturdayRoster ?? new Roster(DepartmentRosterVM.Department, employee, roster.StartDate.AddDays(5)), this);
        SundayRoster = new RosterVM(roster.SundayRoster ?? new Roster(DepartmentRosterVM.Department, employee, roster.StartDate.AddDays(6)), this);

        shifts = new ObservableCollection<Shift>(roster.Shifts);

        DeleteEmployeeRosterCommand = new DeleteEmployeeRosterCommand(this);
    }

    public void SubCount(Shift shift) => DepartmentRosterVM.SubCount(shift);

    public void AddCount(Shift shift) => DepartmentRosterVM.AddCount(shift);

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

    public void ApplyShiftRules()
    {
        // Gather rules that apply to this weekly roster, from the employee.
        var rules = Employee.ShiftRules.Where(rule => rule.AppliesToWeek(StartDate)).ToList();

        FillMissingDailyRosters();

        MondayRoster!.ApplyShiftRules(rules);
        TuesdayRoster!.ApplyShiftRules(rules);
        WednesdayRoster!.ApplyShiftRules(rules);
        ThursdayRoster!.ApplyShiftRules(rules);
        FridayRoster!.ApplyShiftRules(rules);
        SaturdayRoster?.ApplyShiftRules(rules);
        SundayRoster?.ApplyShiftRules(rules);
    }

    private void FillMissingDailyRosters()
    {
        if (Department is null || Employee is null) throw new DataException("Employee Roster missing Department or Employee.");

        // Ensure all daily rosters exist.
        MondayRoster ??= new RosterVM(EmployeeRoster.MondayRoster ??= new Roster(Department, Employee, StartDate), this);
        TuesdayRoster ??= new RosterVM(EmployeeRoster.TuesdayRoster ??= new Roster(Department, Employee, StartDate.AddDays(1)), this);
        WednesdayRoster ??= new RosterVM(EmployeeRoster.WednesdayRoster ??= new Roster(Department, Employee, StartDate.AddDays(2)), this);
        ThursdayRoster ??= new RosterVM(EmployeeRoster.ThursdayRoster ??= new Roster(Department, Employee, StartDate.AddDays(3)), this);
        FridayRoster ??= new RosterVM(EmployeeRoster.FridayRoster ??= new Roster(Department, Employee, StartDate.AddDays(4)), this);
        if (UseSaturdays) SaturdayRoster ??= new RosterVM(EmployeeRoster.SaturdayRoster ??= new Roster(Department, Employee, StartDate.AddDays(5)), this);
        if (UseSundays) SundayRoster ??= new RosterVM(EmployeeRoster.SundayRoster ??= new Roster(Department, Employee, StartDate.AddDays(6)), this);
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

    /*
    public void AddRoster(Roster roster)//, DailyRosterVM dailyRoster)
    {
        var rvm = new RosterVM(roster);//, DepartmentRosterVM, dailyRoster, this);
        rosterVMs.Add(roster.Date, rvm);

        switch (roster.Day)
        {
            case DayOfWeek.Monday:
                MondayRoster = rvm;
                break;
            case DayOfWeek.Tuesday:
                TuesdayRoster = rvm;
                break;
            case DayOfWeek.Wednesday:
                WednesdayRoster = rvm;
                break;
            case DayOfWeek.Thursday:
                ThursdayRoster = rvm;
                break;
            case DayOfWeek.Friday:
                FridayRoster = rvm;
                break;
            case DayOfWeek.Saturday:
                SaturdayRoster = rvm;
                break;
            case DayOfWeek.Sunday:
                SundayRoster = rvm;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(roster.Day), roster.Day, "Unaccounted day of the week.");
        }
    }*/

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
            rosterVM.Type = type;
    }

    public void SetShift(Shift? shift)
    {
        foreach (var rosterVM in RosterVMs())
            rosterVM.SelectedShift = shift;
    }

    private IEnumerable<RosterVM> RosterVMs()
    {
        if (Department is null) return Enumerable.Empty<RosterVM>();

        var returnList = new List<RosterVM>
        {
            (MondayRoster ??= MondayRoster ??= new RosterVM(EmployeeRoster.MondayRoster ??= new Roster(Department, Employee, StartDate), this)),
            (TuesdayRoster ??= new RosterVM(EmployeeRoster.TuesdayRoster ??= new Roster(Department, Employee, StartDate.AddDays(1)), this)),
            (WednesdayRoster ??= new RosterVM(EmployeeRoster.WednesdayRoster ??= new Roster(Department, Employee, StartDate.AddDays(2)), this)),
            (ThursdayRoster ??= new RosterVM(EmployeeRoster.ThursdayRoster ??= new Roster(Department, Employee, StartDate.AddDays(3)), this)),
            (FridayRoster ??= new RosterVM(EmployeeRoster.FridayRoster ??= new Roster(Department, Employee, StartDate.AddDays(4)), this))
        };
        if (UseSaturdays) returnList.Add(SaturdayRoster ??= new RosterVM(EmployeeRoster.SaturdayRoster ??= new Roster(Department, Employee, StartDate.AddDays(5)), this));
        if (UseSundays) returnList.Add(SundayRoster ??= new RosterVM(EmployeeRoster.SundayRoster ??= new Roster(Department, Employee, StartDate.AddDays(6)), this));

        return returnList;
    }

    public void SubShift(Shift rosterShift, DateTime date) => DepartmentRosterVM.SubShift(rosterShift, date);

    public void AddShift(Shift rosterShift, DateTime date) => DepartmentRosterVM.AddShift(rosterShift, date);

    public void Delete()
    {
        // TODO: remove this and all sub-objects from everywhere.
    }

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