using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Model;

public class ShiftRuleRoster : ShiftRule
{
    public string? ShiftID { get; set; }
    public bool Monday { get; set; }
    public bool Tuesday { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday { get; set; }
    public bool Friday { get; set; }
    public bool Saturday { get; set; }
    public bool Sunday { get; set; }

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

    /// <summary>
    /// Compares two roster rules to check if their rotation is harmonious - making sure that they don't clash in any
    /// way assuming they were both to be applied to the same employee.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsHarmoniousRotation(ShiftRuleRoster other)
        => EmployeeID == other.EmployeeID &&
           Rotation && other.Rotation &&
           WeekRotation == other.WeekRotation && FromDate == other.FromDate &&
           WeekNumbers is not null && other.WeekNumbers is not null &&
           !WeekNumbers.Intersect(other.WeekNumbers).Any();

}