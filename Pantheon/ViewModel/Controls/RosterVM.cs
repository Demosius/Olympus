using Pantheon.Annotations;
using System;
using System.Collections.ObjectModel;
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

    public Shift? SelectedShift
    {
        get => Roster.Shift;
        set
        {   
            if (AtWork && Roster.Shift is not null) DailyRosterVM.SubCount(Roster.Shift);
            Roster.Shift = value;
            OnPropertyChanged(nameof(SelectedShift));
            if (AtWork && Roster.Shift is not null) DailyRosterVM.AddCount(Roster.Shift);

            SetShift(value);
        }
    }

    public Employee? Employee
    {
        get => Roster.Employee;
        set
        {
            Roster.Employee = value;
            OnPropertyChanged(nameof(Employee));
        }
    }

    public DateTime Date
    {
        get => Roster.Date;
        set
        {
            Roster.Date = value;
            OnPropertyChanged(nameof(Date));
        }
    }

    public ERosterType Type
    {
        get => Roster.RosterType;
        set
        {
            Roster.RosterType = value;
            OnPropertyChanged(nameof(Type));
            AtWork = Roster.RosterType == ERosterType.Standard;
        }
    }

    public string StartTime
    {
        get => $"{Roster.StartTime.Hours:00}:{Roster.StartTime.Minutes:00}";
        set
        {
            if (!TimeSpan.TryParse(value, out var t)) return;

            Roster.StartTime = t;
            OnPropertyChanged(nameof(StartTime));
        }
    }

    public string EndTime
    {
        get => $"{Roster.EndTime.Hours:00}:{Roster.EndTime.Minutes:00}";
        set
        {
            if (!TimeSpan.TryParse(value, out var t)) return;

            Roster.EndTime = t;
            OnPropertyChanged(nameof(EndTime));
        }
    }

    public bool AtWork
    {
        get => Roster.AtWork;
        set
        {
            var shiftCount = value != Roster.AtWork;
            Roster.AtWork = value;
            OnPropertyChanged(nameof(AtWork));

            if (!shiftCount || SelectedShift is null) return;

            if (AtWork) 
                DailyRosterVM.AddCount(SelectedShift);
            else
                DailyRosterVM.SubCount(SelectedShift);
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

        shifts = new ObservableCollection<Shift>(EmployeeRosterVM.Shifts);

        if (SelectedShift is not null && AtWork) dailyRosterVM.AddCount(SelectedShift);
    }

    public void SetShift(Shift? shift)
    {
        Roster.Shift = shift;
        Roster.StartTime = shift?.StartTime ?? TimeSpan.Zero; 
        Roster.EndTime = shift?.EndTime ?? TimeSpan.Zero;
        OnPropertyChanged(nameof(StartTime));
        OnPropertyChanged(nameof(EndTime));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}