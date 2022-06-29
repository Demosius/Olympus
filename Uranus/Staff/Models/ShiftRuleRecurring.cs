using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Models;

public enum ERecurringRuleType
{
    Away,
    LeaveEarly,
    ArriveLate,
    SetShift,
}

public enum EStandardRotations
{
    Weekly = 1,
    Fortnightly = 2,
    ThreeWeeks = 3,
    FourWeeks = 4,
    FiveWeeks = 5,
    SixWeeks = 6,
    SevenWeeks = 7,
    EightWeeks = 8,
    NineWeeks = 9,
    TenWeeks = 10,
    ElevenWeeks = 11,
    TwelveWeeks = 12
}

public class ShiftRuleRecurring : ShiftRule
{
    public DayOfWeek DayOfWeek { get; set; }
    public int WeekRotation { get; set; }
    public string WeekNumbers { get; set; }
    public DateTime FromDate { get; set; }
    public TimeSpan? TimeOfDay { get; set; }
    public ERecurringRuleType RuleType { get; set; }
    public string ShiftID { get; set; }

    [ManyToOne(nameof(EmployeeID), nameof(Models.Employee.RecurringRules), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }
    [ManyToOne(nameof(ShiftID), CascadeOperations = CascadeOperation.None)]
    public Shift? Shift { get; set; }

    [Ignore]
    public List<int> WeekNumberList
    {
        get => WeekNumbers == "" ? new List<int>() : WeekNumbers.Split(",").Select(int.Parse).ToList();
        set
        {
            value.Sort();
            WeekNumbers = string.Join(",", value);
        }
    }

    [Ignore]
    public string Summary
    {
        get
        {
            return RuleType switch
            {
                ERecurringRuleType.Away => WeekRotation == 1
                    ? $"Away every {DayOfWeek}."
                    : $"Away on {DayOfWeek}. Week{(WeekNumberList.Count > 1 ? "s" : "")}: {WeekNumbers} on {WeekRotation} week rotation.",
                ERecurringRuleType.ArriveLate => WeekRotation == 1
                    ? $"Arrive late ({TimeOfDay}) every {DayOfWeek}."
                    : $"Arrive late ({TimeOfDay}) on {DayOfWeek}. Week{(WeekNumberList.Count > 1 ? "s" : "")}: {WeekNumbers} on {WeekRotation} week rotation.",
                ERecurringRuleType.LeaveEarly => WeekRotation == 1
                    ? $"Leave early ({TimeOfDay}) every {DayOfWeek}."
                    : $"Leave early ({TimeOfDay}) on {DayOfWeek}. Week{(WeekNumberList.Count > 1 ? "s" : "")}: {WeekNumbers} on {WeekRotation} week rotation.",
                ERecurringRuleType.SetShift => WeekRotation == 1
                ? $"Set shift: {ShiftID}. Every {DayOfWeek}."
                : $"Set shift: {ShiftID}. {DayOfWeek}s for week{(WeekNumberList.Count > 1 ? "s" : "")}: {WeekNumbers} on {WeekRotation} week rotation.",
                _ => throw new ArgumentOutOfRangeException($"{RuleType} rule type not acounted for.")
            };
        }
    }

    public ShiftRuleRecurring()
    {
        WeekNumbers = string.Empty;
        ShiftID = string.Empty;
        FromDate = DateTime.Now.Date;
        DayOfWeek = FromDate.DayOfWeek;
    }

    public ShiftRuleRecurring(Employee employee) : this()
    {
        Employee = employee;
        EmployeeID = employee.ID;
    }

    public ShiftRuleRecurring Copy()
    {
        return new ShiftRuleRecurring
        {
            ID = ID,
            Shift = Shift,
            FromDate = FromDate,
            DayOfWeek = DayOfWeek,
            RuleType = RuleType,
            ShiftID = ShiftID,
            WeekRotation = WeekRotation,
            WeekNumberList = WeekNumberList,
            WeekNumbers = WeekNumbers,
            TimeOfDay = TimeOfDay,
            Employee = Employee,
            EmployeeID = EmployeeID,
            Description = Description
        };
    }

    public override bool AppliesToWeek(DateTime weeksStartDate)
    {
        if (WeekRotation is 1 or 0) return true;
        if (FromDate > weeksStartDate) return false;
        var weeksFromStart = (weeksStartDate - FromDate).Days / 7;
        var weekInRotation = weeksFromStart % WeekRotation;
        return WeekNumbers.Split(",").ToList().Contains(weekInRotation.ToString());
    }

    public override bool AppliesToDay(DateTime date) => DayOfWeek == date.DayOfWeek &&
                                                        AppliesToWeek(date.AddDays(-(date.DayOfWeek - DayOfWeek.Monday)));
}