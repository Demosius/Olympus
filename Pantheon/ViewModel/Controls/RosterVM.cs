using System;
using System.Collections.ObjectModel;
using Pantheon.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Controls;

internal class RosterVM : INotifyPropertyChanged
{
    public Roster Roster { get; set; }

    public DepartmentRosterVM DepartmentRosterVM { get; set; }
    public DailyRosterVM DailyRosterVM { get; set; }
    public EmployeeRosterVM EmployeeRosterVM { get; set; }

    #region INotifyPropertyChanged Members
    
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

    private DateTime date;
    public DateTime Date
    {
        get => date;
        set
        {
            date = value;
            OnPropertyChanged(nameof(Date));
        }
    }

    private ERosterType type;
    public ERosterType Type
    {
        get => type;
        set
        {
            type = value;
            OnPropertyChanged(nameof(Type));
        }
    }

    #endregion

    public RosterVM(Roster roster, DepartmentRosterVM departmentRosterVM, DailyRosterVM dailyRosterVM, EmployeeRosterVM employeeRosterVM)
    {
        Roster = roster;
        Date = roster.Date;
        DepartmentRosterVM = departmentRosterVM;
        DailyRosterVM = dailyRosterVM;
        EmployeeRosterVM = employeeRosterVM;
        employee = EmployeeRosterVM.Employee;

        shifts = new ObservableCollection<Shift>(EmployeeRosterVM.Shifts);

        selectedShift = roster.Shift;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}