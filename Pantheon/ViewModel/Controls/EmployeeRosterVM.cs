using Pantheon.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Controls;

internal class EmployeeRosterVM : INotifyPropertyChanged
{
    public EmployeeRoster EmployeeRoster { get; set; }
    private Dictionary<DateTime, RosterVM> rosterVMs = new();

    #region INotifyPropertyChanged Members

    private RosterVM? mondayRoster;
    public RosterVM? MondayRoster
    {
        get => mondayRoster;
        set
        {
            mondayRoster = value;
            OnPropertyChanged(nameof(MondayRoster));
        }
    }

    private RosterVM? tuesdayRoster;
    public RosterVM? TuesdayRoster
    {
        get => tuesdayRoster;
        set
        {
            tuesdayRoster = value;
            OnPropertyChanged(nameof(TuesdayRoster));
        }
    }

    private RosterVM? wednesdayRoster;
    public RosterVM? WednesdayRoster
    {
        get => wednesdayRoster;
        set
        {
            wednesdayRoster = value;
            OnPropertyChanged(nameof(WednesdayRoster));
        }
    }

    private RosterVM? thursdayRoster;
    public RosterVM? ThursdayRoster
    {
        get => thursdayRoster;
        set
        {
            thursdayRoster = value;
            OnPropertyChanged(nameof(ThursdayRoster));
        }
    }

    private RosterVM? fridayRoster;
    public RosterVM? FridayRoster
    {
        get => fridayRoster;
        set
        {
            fridayRoster = value;
            OnPropertyChanged(nameof(FridayRoster));
        }
    }

    private RosterVM? saturdayRoster;
    public RosterVM? SaturdayRoster
    {
        get => saturdayRoster;
        set
        {
            saturdayRoster = value;
            OnPropertyChanged(nameof(SaturdayRoster));
        }
    }

    private RosterVM? sundayRoster;
    public RosterVM? SundayRoster
    {
        get => sundayRoster;
        set
        {
            sundayRoster = value;
            OnPropertyChanged(nameof(SundayRoster));
        }
    }

    private Employee employee;
    public Employee Employee
    {
        get => employee;
        set
        {
            employee = value;
            OnPropertyChanged(nameof(Employee));
        }
    }

    private ObservableCollection<Shift> shifts;
    public ObservableCollection<Shift> Shifts
    {
        get => shifts;
        set
        {
            shifts = value;
            OnPropertyChanged(nameof(Shifts));
        }
    }

    private Shift? selectedShift;
    public Shift? SelectedShift
    {
        get => selectedShift;
        set
        {
            selectedShift = value;
            OnPropertyChanged(nameof(SelectedShift));

            EmployeeRoster.Shift = selectedShift;
            foreach (var (_, rosterVM) in rosterVMs)
                rosterVM.SelectedShift = selectedShift;
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

        selectedShift = roster.Shift;

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

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}