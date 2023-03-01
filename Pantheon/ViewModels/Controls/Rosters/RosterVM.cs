using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Pantheon.Annotations;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Rosters;

public class RosterVM : INotifyPropertyChanged
{
    public Roster Roster { get; set; }

    public EmployeeRosterVM EmployeeRosterVM { get; set; }

    #region Roster Access

    public Shift? SelectedShift
    {
        get => Roster.Shift;
        set
        {
            if (AtWork && Roster.Shift is not null) Roster.SubCount(Roster.Shift);
            Roster.Shift = value;
            Roster.ShiftID = Roster.Shift?.ID ?? "";
            OnPropertyChanged();
            if (AtWork && Roster.Shift is not null) Roster.AddCount(Roster.Shift);

            SetShift(value);
        }
    }

    public Employee? Employee  => Roster.Employee;

    public DateTime Date => Roster.Date;

    public ERosterType Type
    {
        get => Roster.RosterType;
        set
        {
            Roster.RosterType = value;
            OnPropertyChanged();
            AtWork = Roster.RosterType == ERosterType.Standard;
            if (Roster.RosterType != ERosterType.PublicHoliday || (Roster.DailyRoster?.IsPublicHoliday ?? false)) return;
            PromptPublicHoliday();
        }
    }

    public string StartTime
    {
        get => $"{Roster.StartTime.Hours:00}:{Roster.StartTime.Minutes:00}";
        set
        {
            if (!TimeSpan.TryParse(value, out var t)) return;

            Roster.StartTime = t;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DisplayString));
        }
    }

    public string EndTime
    {
        get => $"{Roster.EndTime.Hours:00}:{Roster.EndTime.Minutes:00}";
        set
        {
            if (!TimeSpan.TryParse(value, out var t)) return;

            Roster.EndTime = t;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DisplayString));
        }
    }

    public bool AtWork
    {
        get => Roster.AtWork;
        set
        {
            var shiftCount = value != Roster.AtWork;
            Roster.AtWork = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(NotAtWork));
            OnPropertyChanged(nameof(DisplayString));

            if (!shiftCount || SelectedShift is null) return;

            if (AtWork)
                Roster.AddCount(SelectedShift);
            else
                Roster.SubCount(SelectedShift);
        }
    }

    public bool NotAtWork => !AtWork;

    #endregion

    #region Parent (EmployeeRoster) VM Access

    public DepartmentRosterVM DepartmentRosterVM => EmployeeRosterVM.DepartmentRosterVM;

    #endregion

    #region INotifyPropertyChanged Members

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

    public string DisplayString => ToString();
    
    #endregion

    public RosterVM(Roster roster, EmployeeRosterVM employeeRosterVM)
    {
        Roster = roster;
        shifts = new ObservableCollection<Shift>(Roster.Shifts);
        EmployeeRosterVM = employeeRosterVM;

        if (SelectedShift is not null && AtWork) Roster.AddCount(SelectedShift);
    }

    public void SetShift(Shift? shift)
    {
        // Pass information up chain for Tracking Counts of Shifts.
        if (Roster.Shift is not null) EmployeeRosterVM.SubShift(Roster.Shift, Date);
        // Change Actual Shift and data.
        Roster.Shift = shift;
        Roster.StartTime = shift?.StartTime ?? TimeSpan.Zero;
        Roster.EndTime = shift?.EndTime ?? TimeSpan.Zero;
        // Make sure changes are registered in display.
        OnPropertyChanged(nameof(StartTime));
        OnPropertyChanged(nameof(EndTime));
        OnPropertyChanged(nameof(DisplayString));
        // Pass information tracking up chain.
        if (Roster.Shift is not null) EmployeeRosterVM.AddShift(Roster.Shift, Date);
    }

    /// <summary>
    /// Checks with the user if this date is to be set as a public holiday for all.
    /// </summary>
    private void PromptPublicHoliday()
    {
        if (MessageBox.Show($"Do you want to set {Date:dddd, dd/MM/yyyy} as a public holiday?", "Public Holiday",
                MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
            Roster.DailyRoster?.SetPublicHoliday();
    }

    /// <summary>
    /// Sets the roster type as public holiday without using the Type Setter - which would result in recursive prompting.
    /// </summary>
    public void SetPublicHoliday(bool isPublicHoliday = true)
    {
        Roster.RosterType = isPublicHoliday ? ERosterType.PublicHoliday : ERosterType.Standard;
        OnPropertyChanged(nameof(Type));
        if (isPublicHoliday)
            AtWork = false;
        else
            AtWork = Roster.DailyRoster?.Day is not (DayOfWeek.Saturday or DayOfWeek.Sunday) || Roster.Shift is not null;
    }

    /*public void ApplyShiftRules(List<ShiftRule> rules) => Roster.ApplyShiftRules(rules);*/
    /*{
        ShiftRules.AddRange(rules.Where(rule => rule.AppliesToDay(Date)));
        ShiftRules = ShiftRules.Distinct().ToList();
    }*/

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return Roster.ToString();
    }
}