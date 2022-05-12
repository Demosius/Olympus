using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using SQLite;

namespace Uranus.Staff.Model;

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
    [TextBlob(nameof(WeekNumbersBlob))]
    public List<int>? WeekNumbers { get; set; }
    public string? WeekNumbersBlob { get; set; }
    public DateTime FromDate { get; set; }
    public TimeSpan? TimeOfDay { get; set; }
    public ERecurringRuleType RuleType { get; set; }
    public string ShiftID { get; set; }

    [ManyToOne(nameof(EmployeeID), nameof(Model.Employee.RecurringRules), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }
    [ManyToOne(nameof(ShiftID), CascadeOperations = CascadeOperation.None)]
    public Shift? Shift { get; set; }
    
    [Ignore] public string Summary => $"Every {WeekNumbers} of {WeekRotation} weeks on {DayOfWeek} - {RuleType}.";

    public ShiftRuleRecurring()
    {
        ShiftID = string.Empty;
        FromDate = DateTime.Now.Date;
        DayOfWeek = FromDate.DayOfWeek;
    }

    public ShiftRuleRecurring(Employee employee) : this()
    {
        Employee = employee;
        EmployeeID = employee.ID;
    }
}