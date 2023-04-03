using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Rosters;

public class RosterVM : INotifyPropertyChanged
{
    public Roster Roster { get; set; }

    public EmployeeRosterVM EmployeeRosterVM { get; set; }

    private bool? designatedRoster;

    #region Roster Access

    public Shift? SelectedShift
    {
        get => Roster.Shift;
        set
        {
            if (AtWork && Roster.Shift is not null) SubCount(Roster.Shift);
            Roster.Shift = value;
            Roster.ShiftID = Roster.Shift?.ID ?? "";
            OnPropertyChanged();
            if (AtWork && Roster.Shift is not null) AddCount(Roster.Shift);

            SetShift(value);
            SetDisplayString();
        }
    }

    public List<ShiftRule> ShiftRules
    {
        get => Roster. ShiftRules; 
        set => Roster.ShiftRules = value;
    }

    public Employee? Employee  => Roster.Employee;

    public DateTime Date => Roster.Date;

    public DayOfWeek Day => Roster.Day;

    public ERosterType Type
    {
        get => Roster.RosterType;
        set
        {
            Roster.RosterType = value;
            OnPropertyChanged();
            AtWork = Roster.RosterType == ERosterType.Standard;
            if (Roster.RosterType != ERosterType.PublicHoliday || EmployeeRosterVM.IsPublicHoliday(Date)) return;
            PromptPublicHoliday();
            SetDisplayString();
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
            SetDisplayString();
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
            SetDisplayString();
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
            SetDisplayString();

            if (!shiftCount || SelectedShift is null) return;

            if (AtWork)
                AddCount(SelectedShift);
            else
                SubCount(SelectedShift);
        }
    }

    public bool NotAtWork => !AtWork;

    #endregion

    #region Parent (EmployeeRoster) VM Access

    public DepartmentRosterVM DepartmentRosterVM => EmployeeRosterVM.DepartmentRosterVM;

    public bool IsPublicHoliday => EmployeeRosterVM.IsPublicHoliday(Date);

    public int DaysInUse => EmployeeRosterVM.DaysInUse;

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

    private string displayString;
    public string DisplayString
    {
        get => displayString;
        set
        {
            displayString = value;
            OnPropertyChanged();
            EmployeeRosterVM.NotifyDailies();
        }
    }
    
    #endregion

    public RosterVM(Roster roster, EmployeeRosterVM employeeRosterVM)
    {
        Roster = roster;
        shifts = new ObservableCollection<Shift>(Roster.Shifts);
        EmployeeRosterVM = employeeRosterVM;

        displayString = roster.ToString();

        if (SelectedShift is not null && AtWork) AddCount(SelectedShift);
    }

    public void SubCount(Shift shift) => EmployeeRosterVM.SubDailyCount(shift, Day);

    public void AddCount(Shift shift) => EmployeeRosterVM.AddDailyCount(shift, Day);

    public void ApplyShiftRules(List<ShiftRule> rules)
    {
        ShiftRules.AddRange(rules.Where(rule => rule.AppliesToDay(Date)));
        ShiftRules = ShiftRules.Distinct().ToList();

        // Apply in order: Roster, Recurring, Single. 
        // Reverse order of priority so that the higher priority can overwrite the lower.
        var rosterRules = ShiftRules.OfType<ShiftRuleRoster>();
        var recurringRules = ShiftRules.OfType<ShiftRuleRecurring>();
        var singles = ShiftRules.OfType<ShiftRuleSingle>();

        // There shouldn't be clashing roster rules relevant to the day, but if there is let it be random as to which applies. 
        foreach (var rosterRule in rosterRules)
        {
            if (rosterRule.Shift is not null)  SelectedShift = rosterRule.Shift;

            designatedRoster = rosterRule.Day(Day);

            if (Type != ERosterType.PublicHoliday)
                Type = designatedRoster switch
                {
                    true => ERosterType.Standard,
                    false => ERosterType.RDO,
                    null => Type
                };
        }

        foreach (var recurringRule in recurringRules)
        {
            switch (recurringRule.RuleType)
            {
                case ERecurringRuleType.Away:
                    if (Type != ERosterType.PublicHoliday)
                    {
                        Type = ERosterType.RDO;
                        designatedRoster = false;
                    }
                    break;
                case ERecurringRuleType.LeaveEarly:
                    if (SelectedShift is not null)
                    {
                        Type = ERosterType.Standard;
                        EndTime = recurringRule.TimeOfDay?.ToString() ?? EndTime;
                    }
                    break;
                case ERecurringRuleType.ArriveLate:
                    if (SelectedShift is not null)
                    {
                        Type = ERosterType.Standard;
                        StartTime = recurringRule.TimeOfDay?.ToString() ?? StartTime;
                    }
                    break;
                case ERecurringRuleType.SetShift:
                    SelectedShift = recurringRule.Shift;
                    Type = ERosterType.Standard;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        foreach (var singleRule in singles)
        {
            switch (singleRule.RuleType)
            {
                case ESingleRuleType.Away:
                    if (Type != ERosterType.PublicHoliday)
                    {
                        Type = (ERosterType)singleRule.LeaveType;
                        designatedRoster = false;
                    }
                    break;
                case ESingleRuleType.ArriveLate:
                    if (SelectedShift is not null)
                    {
                        Type = ERosterType.Standard;
                        StartTime = singleRule.Time ?.ToString() ?? StartTime;
                    }
                    break;
                case ESingleRuleType.LeaveEarly:
                    if (SelectedShift is not null)
                    {
                        Type = ERosterType.Standard;
                        EndTime = singleRule.Time?.ToString() ?? EndTime;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public int AdjustShift(double targetShift)
    {
        if (designatedRoster is not null || SelectedShift is null) return 0;
        var counter = GetDailyCounter();
        if (counter is null || counter.OnTarget) return 0;
        if ((counter.UnderTarget && (targetShift < 0 || AtWork)) ||
            (counter.OverTarget && (targetShift > 0 || !AtWork))) return 0;

        if (targetShift < 0)
        {
            Type = ERosterType.RDO;
            return -1;
        }

        Type = ERosterType.Standard;
        return 1;
    }

    private DailyCounterVM? GetDailyCounter()
    {
        var vm = GetDailyRosterVM();
        if (vm is null || SelectedShift is null) return null;
        return vm.ShiftCounter(SelectedShift);
    }

    private DailyRosterVM? GetDailyRosterVM() => EmployeeRosterVM.GetDailyRoster(Day);

    public void SetShift(Shift? shift)
    {
        // Pass information up chain for Tracking Counts of Shifts.
        if (Roster.Shift is not null) SubCount(Roster.Shift);
        // Change Actual Shift and data.
        Roster.Shift = shift;
        Roster.StartTime = shift?.StartTime ?? TimeSpan.Zero;
        Roster.EndTime = shift?.EndTime ?? TimeSpan.Zero;
        // Make sure changes are registered in display.
        OnPropertyChanged(nameof(StartTime));
        OnPropertyChanged(nameof(EndTime));
        OnPropertyChanged(nameof(DisplayString));
        // Pass information tracking up chain.
        if (Roster.Shift is not null) AddCount(Roster.Shift);
    }

    /// <summary>
    /// Checks with the user if this date is to be set as a public holiday for all.
    /// </summary>
    private void PromptPublicHoliday() => EmployeeRosterVM.PromptPublicHoliday(Date);

    /// <summary>
    /// Sets the roster type as public holiday without using the Type Setter - which would result in recursive prompting.
    /// </summary>
    public void SetPublicHoliday(bool isPublicHoliday = true)
    {
        Roster.RosterType = isPublicHoliday ? ERosterType.PublicHoliday : Type == ERosterType.PublicHoliday ? ERosterType.RDO : Type;
        OnPropertyChanged(nameof(Type));
        if (isPublicHoliday)
            AtWork = false;
        else
            AtWork = Roster.DailyRoster?.Day is not (DayOfWeek.Saturday or DayOfWeek.Sunday) && Roster.Shift is not null && Type == ERosterType.Standard;
    }

    /*public void ApplyShiftRules(List<ShiftRule> rules) => Roster.ApplyShiftRules(rules);*/
    /*{
        ShiftRules.AddRange(rules.Where(rule => rule.AppliesToDay(Date)));
        ShiftRules = ShiftRules.Distinct().ToList();
    }*/

    public void SetDisplayString() => DisplayString = Roster.ToString();

    public void Delete() => Roster.Delete();

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return DisplayString;
    }
}