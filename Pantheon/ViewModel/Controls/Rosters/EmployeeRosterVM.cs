using Pantheon.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Controls.Rosters;

public class EmployeeRosterVM : INotifyPropertyChanged
{
    public EmployeeRoster EmployeeRoster { get; set; }
    private readonly Dictionary<DateTime, RosterVM> rosterVMs = new();

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
            if (SelectedShift is not null && SelectedRosterType == ERosterType.Standard) DepartmentRosterVM.SubCount(SelectedShift);
            EmployeeRoster.Shift = value;
            EmployeeRoster.ShiftID = value?.ID ?? "";
            OnPropertyChanged();
            if (SelectedShift is not null && SelectedRosterType == ERosterType.Standard) DepartmentRosterVM.AddCount(SelectedShift);
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
                DepartmentRosterVM.AddCount(SelectedShift);
            else
                DepartmentRosterVM.SubCount(SelectedShift);
        }
    }

    #endregion

    public DepartmentRosterVM DepartmentRosterVM { get; set; }

    public EmployeeRosterVM(EmployeeRoster roster, DepartmentRosterVM departmentRosterVM)
    {
        if (roster.Employee is null) throw new DataException("Employee Roster missing Employee Value.");

        EmployeeRoster = roster;
        employee = EmployeeRoster.Employee;
        DepartmentRosterVM = departmentRosterVM;
        shifts = new ObservableCollection<Shift>();

        foreach (var shift in Employee.Shifts)
            shifts.Add(shift);
    }

    public void AddRoster(Roster roster, DailyRosterVM dailyRoster)
    {
        var rvm = new RosterVM(roster, DepartmentRosterVM, dailyRoster, this);
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
    }

    public void SetRosterType(ERosterType type)
    {
        foreach (var rosterVM in rosterVMs.Values.Where(rosterVM => rosterVM.Date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday)))
            rosterVM.Type = type;
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

    public void SetShift(Shift? shift)
    {
        foreach (var rosterVM in rosterVMs.Values.Where(rosterVM => rosterVM.Date.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday)))
            rosterVM.SelectedShift = shift;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}