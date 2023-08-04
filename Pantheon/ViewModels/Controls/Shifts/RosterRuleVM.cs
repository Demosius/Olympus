using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Pantheon.ViewModels.Interfaces;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Shifts;

public class RosterRuleVM : INotifyPropertyChanged, IShiftRuleVM
{
    public ShiftRuleRoster? SampleRosterRule { get; set; }

    public bool InEdit { get; set; }
    public bool IsNew => !InEdit;

    public bool IsValid => Description != "" &&
                           RequiredMaxDays() >= MaxDays && 
                           RequiredMinDays() <= MinDays &&
                           !(SetShift && Shift is null) &&
                           (SampleRosterRule is null || RosterRule.IsHarmoniousRotation(SampleRosterRule));
    
    public ShiftRuleRoster? Original { get; set; }

    public ShiftRuleRoster RosterRule { get; set; }

    // All shifts
    public ObservableCollection<Shift> Shifts { get; set; }

    private readonly Regex clearRex = new("[^0-9,]+");

    #region INotifyPropertyChanged Members

    public static ObservableCollection<int> DayRange => new(Enumerable.Range(0, 8));

    public string Description
    {
        get => RosterRule.Description;
        set
        {
            RosterRule.Description = value;
            OnPropertyChanged();
        }
    }

    public bool? Monday
    {
        get => RosterRule.Monday;
        set
        {
            RosterRule.Monday = value;
            OnPropertyChanged();
            CheckMinMax();
        }
    }

    public bool? Tuesday
    {
        get => RosterRule.Tuesday;
        set
        {
            RosterRule.Tuesday = value;
            OnPropertyChanged();
            CheckMinMax();
        }
    }

    public bool? Wednesday
    {
        get => RosterRule.Wednesday;
        set
        {
            RosterRule.Wednesday = value;
            OnPropertyChanged();
            CheckMinMax();
        }
    }

    public bool? Thursday
    {
        get => RosterRule.Thursday;
        set
        {
            RosterRule.Thursday = value;
            OnPropertyChanged();
            CheckMinMax();
        }
    }

    public bool? Friday
    {
        get => RosterRule.Friday;
        set
        {
            RosterRule.Friday = value;
            OnPropertyChanged();
            CheckMinMax();
        }
    }

    public bool? Saturday
    {
        get => RosterRule.Saturday;
        set
        {
            RosterRule.Saturday = value;
            OnPropertyChanged();
            CheckMinMax();
        }
    }

    public bool? Sunday
    {
        get => RosterRule.Sunday;
        set
        {
            RosterRule.Sunday = value;
            OnPropertyChanged();
            CheckMinMax();
        }
    }

    public int MinDays
    {
        get => RosterRule.MinDays;
        set
        {
            RosterRule.MinDays = value < MaxDays ? value : MaxDays;
            CheckMinMax();
            OnPropertyChanged();
        }
    }

    public int MaxDays
    {
        get => RosterRule.MaxDays;
        set
        {
            RosterRule.MaxDays = value > MinDays ? value : MinDays;
            CheckMinMax();
            OnPropertyChanged();
        }
    }

    public bool Rotation
    {
        get => RosterRule.Rotation;
        set
        {
            RosterRule.Rotation = value;
            if (RosterRule.Rotation)
                FromDate ??= DateTime.Now.Date;
            else
                FromDate = null;
            OnPropertyChanged();
        }
    }
    // If using rotation.
    public DateTime? FromDate
    {
        get => RosterRule.FromDate;
        set
        {
            if (value is null)
                RosterRule.FromDate = value;
            else // Make sure the date is set to a monday.
            {
                var date = (DateTime)value;
                RosterRule.FromDate = date.AddDays(DayOfWeek.Monday - date.DayOfWeek);
            }
            OnPropertyChanged();
        }
    }

    public EStandardRotations WeekRotation
    {
        get => (EStandardRotations)(RosterRule.WeekRotation ?? 1);
        set
        {
            RosterRule.WeekRotation = (int)value;
            OnPropertyChanged();
        }
    }

    public string WeekNumbersText
    {
        get => string.Join(", ", RosterRule.WeekNumberList);
        set
        {
            var clean = clearRex.Replace(value, "");
            if (clean == "") clean = "1";
            var stringArray = clean.Split('\u002C');
            var days = stringArray.Select(int.Parse).Where(x => x <= RosterRule.WeekRotation).ToList();
            days.Sort();
            if (days.Count == 0) days.Add(1);
            RosterRule.WeekNumberList = days;
            OnPropertyChanged();
        }
    }
    
    public bool SetShift
    {
        get => RosterRule.SetShift;
        set
        {
            RosterRule.SetShift = value;
            if (!SetShift) Shift = null;
            OnPropertyChanged();
        }
    }

    public Shift? Shift
    {
        get => RosterRule.Shift;
        set
        {
            RosterRule.Shift = value;
            RosterRule.ShiftID = value?.ID ?? string.Empty;
            OnPropertyChanged();
        }
    }

    #endregion

    public RosterRuleVM()
    {
        RosterRule = new ShiftRuleRoster();
        Shifts = new ObservableCollection<Shift>();
    }

    public RosterRuleVM(Employee employee, List<Shift> shifts)
    {
        RosterRule = new ShiftRuleRoster(employee);
        Shifts = new ObservableCollection<Shift>(shifts);
    }

    public RosterRuleVM(Employee employee, ShiftRuleRoster sampleRosterRule, List<Shift> shifts)
    {
        RosterRule = new ShiftRuleRoster(employee, sampleRosterRule);
        SampleRosterRule = sampleRosterRule;
        Shifts = new ObservableCollection<Shift>(shifts);
    }

    public RosterRuleVM(ShiftRuleRoster rosterRule, List<Shift> shifts)
    {
        InEdit = true;
        Original = rosterRule;
        RosterRule = rosterRule.Copy();
        Shifts = new ObservableCollection<Shift>(shifts);
    }

    public void CheckMinMax()
    {
        var min = RequiredMinDays();
        var max = RequiredMaxDays();

        if (min > max) throw new DataException("Impossible restrictions met. Max required days lower than min.");

        if (RosterRule.MinDays < min)
        {
            RosterRule.MinDays = min;
            OnPropertyChanged(nameof(MinDays));
        }
        else if (RosterRule.MinDays > max)
        {
            RosterRule.MinDays = max;
            OnPropertyChanged(nameof(MinDays));
        }

        if (RosterRule.MaxDays < min)
        {
            RosterRule.MaxDays = min;
            OnPropertyChanged(nameof(MaxDays));
        }
        else if (RosterRule.MaxDays > max)
        {
            RosterRule.MaxDays = max;
            OnPropertyChanged(nameof(MaxDays));
        }
    }

    /// <summary>
    /// Based on the attendance selection for each day.
    /// </summary>
    /// <returns></returns>
    private int RequiredMinDays()
    {
        var count = 0;
        if (Monday == true) ++count;
        if (Tuesday == true) ++count;
        if (Wednesday == true) ++count;
        if (Thursday == true) ++count;
        if (Friday == true) ++count;
        if (Saturday == true) ++count;
        if (Sunday == true) ++count;
        return count;
    }

    /// <summary>
    /// Based on the attendance selection for each day.
    /// </summary>
    /// <returns></returns>
    private int RequiredMaxDays()
    {
        var count = 7;
        if (Monday == false) --count;
        if (Tuesday == false) --count;
        if (Wednesday == false) --count;
        if (Thursday == false) --count;
        if (Friday == false) --count;
        if (Saturday == false) --count;
        if (Sunday == false) --count;
        return count;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}