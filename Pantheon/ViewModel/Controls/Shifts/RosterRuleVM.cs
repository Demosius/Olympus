using Pantheon.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Controls.Shifts;

public class RosterRuleVM : INotifyPropertyChanged
{
    public ShiftRuleRoster? SampleRosterRule { get; set; }

    public bool InEdit { get; set; }
    public bool IsNew => !InEdit;

    public ShiftRuleRoster? Original { get; set; }

    public ShiftRuleRoster ShiftRule { get; set; }

    private readonly Regex clearRex = new("[^0-9,]+");

    #region INotifyPropertyChanged Members

    public ObservableCollection<int> DayRange => new(Enumerable.Range(0, 8));

    public string Description
    {
        get => ShiftRule.Description;
        set
        {
            ShiftRule.Description = value;
            OnPropertyChanged();
        }
    }

    public bool? Monday
    {
        get => ShiftRule.Monday;
        set
        {
            ShiftRule.Monday = value;
            OnPropertyChanged();
            CheckMinMax();
        }
    }

    public bool? Tuesday
    {
        get => ShiftRule.Tuesday;
        set
        {
            ShiftRule.Tuesday = value;
            OnPropertyChanged();
            CheckMinMax();
        }
    }

    public bool? Wednesday
    {
        get => ShiftRule.Wednesday;
        set
        {
            ShiftRule.Wednesday = value;
            OnPropertyChanged();
            CheckMinMax();
        }
    }

    public bool? Thursday
    {
        get => ShiftRule.Thursday;
        set
        {
            ShiftRule.Thursday = value;
            OnPropertyChanged();
            CheckMinMax();
        }
    }

    public bool? Friday
    {
        get => ShiftRule.Friday;
        set
        {
            ShiftRule.Friday = value;
            OnPropertyChanged();
            CheckMinMax();
        }
    }

    public bool? Saturday
    {
        get => ShiftRule.Saturday;
        set
        {
            ShiftRule.Saturday = value;
            OnPropertyChanged();
            CheckMinMax();
        }
    }

    public bool? Sunday
    {
        get => ShiftRule.Sunday;
        set
        {
            ShiftRule.Sunday = value;
            OnPropertyChanged();
            CheckMinMax();
        }
    }

    public int MinDays
    {
        get => ShiftRule.MinDays;
        set
        {
            ShiftRule.MinDays = value < MaxDays ? value : MaxDays;
            CheckMinMax();
            OnPropertyChanged();
        }
    }

    public int MaxDays
    {
        get => ShiftRule.MaxDays;
        set
        {
            ShiftRule.MaxDays = value > MinDays ? value : MinDays;
            CheckMinMax();
            OnPropertyChanged();
        }
    }

    public bool Rotation
    {
        get => ShiftRule.Rotation;
        set
        {
            ShiftRule.Rotation = value;
            if (ShiftRule.Rotation)
                FromDate ??= DateTime.Now.Date;
            else
                FromDate = null;
            OnPropertyChanged();
        }
    }
    // If using rotation.
    public DateTime? FromDate
    {
        get => ShiftRule.FromDate;
        set
        {
            if (value is null)
                ShiftRule.FromDate = value;
            else // Make sure the date is set to a monday.
            {
                var date = (DateTime)value;
                ShiftRule.FromDate = date.AddDays(DayOfWeek.Monday - date.DayOfWeek);
            }
            OnPropertyChanged();
        }
    }

    public EStandardRotations WeekRotation
    {
        get => (EStandardRotations)(ShiftRule.WeekRotation ?? 1);
        set
        {
            ShiftRule.WeekRotation = (int)value;
            OnPropertyChanged();
        }
    }

    public string WeekNumbersText
    {
        get => string.Join(", ", ShiftRule.WeekNumberList);
        set
        {
            var clean = clearRex.Replace(value, "");
            if (clean == "") clean = "1";
            var stringArray = clean.Split('\u002C');
            var days = stringArray.Select(int.Parse).Where(x => x <= ShiftRule.WeekRotation).ToList();
            days.Sort();
            if (days.Count == 0) days.Add(1);
            ShiftRule.WeekNumberList = days;
            OnPropertyChanged();
        }
    }

    private bool setShift;
    public bool SetShift
    {
        get => setShift;
        set
        {
            setShift = value;
            if (!setShift) Shift = null;
            OnPropertyChanged();
        }
    }

    public Shift? Shift
    {
        get => ShiftRule.Shift;
        set
        {
            ShiftRule.Shift = value;
            ShiftRule.ShiftID = value?.ID ?? string.Empty;
            OnPropertyChanged();
        }
    }

    #endregion

    public RosterRuleVM()
    {
        ShiftRule = new ShiftRuleRoster();
    }

    public RosterRuleVM(Employee employee)
    {
        ShiftRule = new ShiftRuleRoster(employee);
    }

    public RosterRuleVM(Employee employee, ShiftRuleRoster sampleRosterRule)
    {
        ShiftRule = new ShiftRuleRoster(employee, sampleRosterRule);
        SampleRosterRule = sampleRosterRule;
    }

    public RosterRuleVM(ShiftRuleRoster rosterRule)
    {
        InEdit = true;
        Original = rosterRule;
        ShiftRule = rosterRule.Copy();
    }

    public void CheckMinMax()
    {
        var min = RequiredMinDays();
        var max = RequiredMaxDays();

        if (min > max) throw new DataException("Impossible restrictions met. Max required days lower than min.");

        if (ShiftRule.MinDays < min)
        {
            ShiftRule.MinDays = min;
            OnPropertyChanged(nameof(MinDays));
        }
        else if (ShiftRule.MinDays > max)
        {
            ShiftRule.MinDays = max;
            OnPropertyChanged(nameof(MinDays));
        }

        if (ShiftRule.MaxDays < min)
        {
            ShiftRule.MaxDays = min;
            OnPropertyChanged(nameof(MaxDays));
        }
        else if (ShiftRule.MaxDays > max)
        {
            ShiftRule.MaxDays = max;
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

    public bool IsValid()
    {
        if (Description == "") return false;
        if (RequiredMaxDays() < MaxDays) return false;
        if (RequiredMinDays() > MinDays) return false;
        if (SetShift && Shift is null) return false;
        if (SampleRosterRule is not null && !ShiftRule.IsHarmoniousRotation(SampleRosterRule)) return false;

        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}