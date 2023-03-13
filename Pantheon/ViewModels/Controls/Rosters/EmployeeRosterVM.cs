using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Rosters;

public class EmployeeRosterVM : INotifyPropertyChanged
{
    public EmployeeRoster EmployeeRoster { get; set; }
    private readonly Dictionary<DateTime, RosterVM> rosterVMs = new();

    public DepartmentRosterVM DepartmentRosterVM { get; set; }

    public bool UseSaturdays => DepartmentRosterVM.UseSaturdays;
    public bool UseSundays => DepartmentRosterVM.UseSundays;

    public string EmployeeName => Employee.FullName;

    public string EmployeeIcon => Employee.Icon?.FullPath ?? string.Empty;

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
            if (SelectedShift is not null && SelectedRosterType == ERosterType.Standard) EmployeeRoster.SubCount(SelectedShift);
            EmployeeRoster.Shift = value;
            EmployeeRoster.ShiftID = value?.ID ?? "";
            OnPropertyChanged();
            if (SelectedShift is not null && SelectedRosterType == ERosterType.Standard) EmployeeRoster.AddCount(SelectedShift);
            SetShift(value);
        }
    }

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
                EmployeeRoster.AddCount(SelectedShift);
            else
                EmployeeRoster.SubCount(SelectedShift);
        }
    }

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

    public void SetRosterType(ERosterType type)
    {
        foreach (var rosterVM in rosterVMs.Values.Where(rosterVM => rosterVM.Date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday)))
            rosterVM.Type = type;
    }

    public void SetShift(Shift? shift)
    {
        foreach (var rosterVM in rosterVMs.Values.Where(rosterVM => rosterVM.Date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday)))
            rosterVM.SelectedShift = shift;
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

    public void SubShift(Shift rosterShift, DateTime date) => DepartmentRosterVM.SubShift(rosterShift, date);

    public void AddShift(Shift rosterShift, DateTime date) => DepartmentRosterVM.AddShift(rosterShift, date);
}