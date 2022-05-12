﻿using Pantheon.Annotations;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Controls.Shifts;

public class SingleRuleVM : INotifyPropertyChanged
{
    public ShiftRuleSingle ShiftRule { get; set; }

    #region INotifyPropertyChanged Memebers

    public string Description
    {
        get => ShiftRule.Description;
        set
        {
            ShiftRule.Description = value;
            OnPropertyChanged();
        }
    }

    public DateTime StartDate
    {
        get => ShiftRule.StartDate;
        set
        {
            ShiftRule.StartDate = value;
            if (ShiftRule.EndDate < ShiftRule.StartDate)
            {
                ShiftRule.EndDate = ShiftRule.StartDate;
                OnPropertyChanged(nameof(EndDate));
            }
            OnPropertyChanged();
        }
    }

    public DateTime EndDate
    {
        get => ShiftRule.EndDate;
        set
        {
            ShiftRule.EndDate = value;
            if (ShiftRule.StartDate > ShiftRule.EndDate)
            {
                ShiftRule.StartDate = ShiftRule.EndDate;
                OnPropertyChanged(nameof(StartDate));
            }
            OnPropertyChanged();
        }
    }

    public TimeSpan? Time
    {
        get => ShiftRule.Time;
        set
        {
            ShiftRule.Time = value;
            OnPropertyChanged();
        }
    }

    public string TimeString
    {
        get => ShiftRule.Time.ToString() ?? "00:00";
        set
        {
            if (TimeSpan.TryParse(value, out var time))
                ShiftRule.Time = time;

            OnPropertyChanged();
            OnPropertyChanged(nameof(Time));
        }
    }

    public ESingleRuleType RuleType
    {
        get => ShiftRule.RuleType;
        set
        {
            ShiftRule.RuleType = value;
            if (RuleType is ESingleRuleType.ArriveLate or ESingleRuleType.LeaveEarly)
                Time ??= new TimeSpan();
            OnPropertyChanged();
            OnPropertyChanged(nameof(UseTime));
            OnPropertyChanged(nameof(UseLeaveType));
        }
    }

    public ELeaveType LeaveType
    {
        get => ShiftRule.LeaveType;
        set
        {
            ShiftRule.LeaveType = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public bool UseTime => ShiftRule.RuleType is ESingleRuleType.ArriveLate or ESingleRuleType.LeaveEarly;

    public bool UseLeaveType => ShiftRule.RuleType is ESingleRuleType.Away;

    public SingleRuleVM()
    {
        ShiftRule = new ShiftRuleSingle();
    }

    public SingleRuleVM(Employee employee)
    {
        ShiftRule = new ShiftRuleSingle(employee);
    }

    public bool IsValid()
    {
        if (Description == "") return false;
        if (StartDate > EndDate ) return false;
        return RuleType is not (ESingleRuleType.ArriveLate or ESingleRuleType.LeaveEarly) || Time is not null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}