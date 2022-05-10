using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Staff.Model;

public class ShiftRuleRecurring : ShiftRule
{
    public enum RecurringRuleType
    {
        Away,
        LeaveEarly,
        ArriveLate,
        SetShift,
    }

    public DayOfWeek DayOfWeek { get; set; }
    public int WeekRotation { get; set; }
    public int WeekNumber { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime TimeOfDay { get; set; }
    public RecurringRuleType RuleType { get; set; }

    [ManyToOne(nameof(EmployeeID), nameof(Model.Employee.RecurringRules), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }
}