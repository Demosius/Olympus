using Pantheon.Annotations;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Shifts;

public class RecurringRuleVM : INotifyPropertyChanged
{
    public ShiftRuleRecurring ShiftRule { get; set; }

    public bool InEdit { get; set; }
    public bool IsNew => !InEdit;

    public ShiftRuleRecurring? Original { get; set; }

    private readonly Regex clearRex = new("[^0-9,]+");

    #region INotifyPropertyChange Members

    public string Description
    {
        get => ShiftRule.Description;
        set
        {
            ShiftRule.Description = value;
            OnPropertyChanged();
        }
    }

    public DayOfWeek DayOfWeek
    {
        get => ShiftRule.DayOfWeek;
        set
        {
            ShiftRule.DayOfWeek = value;
            var date = FromDate.AddDays(FromDate.DayOfWeek - value);
            while (date.DayOfWeek != value) date = date.AddDays(1);
            ShiftRule.FromDate = date;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FromDate));
        }
    }

    public EStandardRotations WeekRotation
    {
        get => (EStandardRotations)ShiftRule.WeekRotation;
        set
        {
            ShiftRule.WeekRotation = (int)value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(UseDate));
            WeekNumbersText = string.Join(", ", ShiftRule.WeekNumberList);
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

    public DateTime FromDate
    {
        get => ShiftRule.FromDate;
        set
        {
            ShiftRule.FromDate = value;
            ShiftRule.DayOfWeek = value.DayOfWeek;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DayOfWeek));
        }
    }

    public TimeSpan? TimeOfDay
    {
        get => ShiftRule.TimeOfDay;
        set
        {
            ShiftRule.TimeOfDay = value;
            OnPropertyChanged();
        }
    }

    public string TimeString
    {
        get => ShiftRule.TimeOfDay.ToString() ?? "";
        set
        {
            if (TimeSpan.TryParse(value, out var time))
                ShiftRule.TimeOfDay = time;

            OnPropertyChanged();
            OnPropertyChanged(nameof(TimeOfDay));
        }
    }

    public ERecurringRuleType RuleType
    {
        get => ShiftRule.RuleType;
        set
        {
            ShiftRule.RuleType = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(UseShift));
            OnPropertyChanged(nameof(UseTime));
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

    public bool UseTime => ShiftRule.RuleType is ERecurringRuleType.ArriveLate or ERecurringRuleType.LeaveEarly;

    public bool UseDate => (int)WeekRotation > 1;

    public bool UseShift => ShiftRule.RuleType == ERecurringRuleType.SetShift;

    public RecurringRuleVM()
    {
        ShiftRule = new ShiftRuleRecurring();
    }

    public RecurringRuleVM(Employee employee)
    {
        ShiftRule = new ShiftRuleRecurring(employee);
    }

    public RecurringRuleVM(ShiftRuleRecurring recurringRule)
    {
        InEdit = true;
        Original = recurringRule;
        ShiftRule = recurringRule.Copy();
    }

    public bool IsValid()
    {
        if (Description == "") return false;
        if (FromDate.DayOfWeek != DayOfWeek) return false;
        if (RuleType is ERecurringRuleType.LeaveEarly or ERecurringRuleType.ArriveLate && TimeOfDay is null) return false;
        return RuleType is not ERecurringRuleType.SetShift || Shift is not null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}