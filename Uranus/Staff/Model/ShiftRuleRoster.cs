using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace Uranus.Staff.Model;

public class ShiftRuleRoster : ShiftRule
{
    public string? ShiftID { get; set; }
    public bool? Monday { get; set; }
    public bool? Tuesday { get; set; }
    public bool? Wednesday { get; set; }
    public bool? Thursday { get; set; }
    public bool? Friday { get; set; }
    public bool? Saturday { get; set; }
    public bool? Sunday { get; set; }

    public int MinDays { get; set; }
    public int MaxDays { get; set; }

    public bool Rotation { get; set; }
    // If using rotation.
    public DateTime? FromDate { get; set; }
    public int? WeekRotation { get; set; }
    [TextBlob(nameof(WeekNumbersBlob))]
    public List<int>? WeekNumbers { get; set; }
    public string? WeekNumbersBlob { get; set; }

    [ManyToOne(nameof(ShiftID), nameof(Model.Shift.RosterRules), CascadeOperations = CascadeOperation.None)]
    public Shift? Shift { get; set; }
    [ManyToOne(nameof(EmployeeID), nameof(Model.Employee.RosterRules), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }

    [Ignore] public string Summary => $"{MinDays} - {MaxDays} every {WeekNumbersBlob ?? "1"} of {WeekRotation ?? 1} week(s).";

    public ShiftRuleRoster()
    {
        Sunday = false;
        Saturday = false;
    }

    public ShiftRuleRoster(Employee employee) : this()
    {
        Employee = employee;
        EmployeeID = employee.ID;
    }

    /// <summary>
    /// Use a sample roster to set initial important values to match - such as start date and week rotation.
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="sampleRosterRule"></param>
    public ShiftRuleRoster(Employee employee, ShiftRuleRoster sampleRosterRule) : this(employee)
    {
        Rotation = sampleRosterRule.Rotation;
        FromDate = sampleRosterRule.FromDate;
    }
    /// <summary>
    /// Compares two roster rules to check if their rotation is harmonious - making sure that they don't clash in any
    /// way assuming they were both to be applied to the same employee.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsHarmoniousRotation(ShiftRuleRoster other)
        => EmployeeID == other.EmployeeID &&
           Rotation && other.Rotation &&
           WeekRotation == other.WeekRotation && WeekRotation > 1 &&
           FromDate == other.FromDate &&
           WeekNumbers is not null && other.WeekNumbers is not null &&
           !WeekNumbers.Intersect(other.WeekNumbers).Any();
}