using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Staff.Model;

public class WeeklyShiftCounter : ShiftCounter
{
    [ForeignKey(typeof(DepartmentRoster))] public Guid RosterID { get; set; }

    [ManyToOne(nameof(RosterID), nameof(DepartmentRoster.ShiftCounters), CascadeOperations = CascadeOperation.None)]
    public DepartmentRoster? Roster { get; set; }

    public WeeklyShiftCounter() { }

    public WeeklyShiftCounter(DepartmentRoster roster, Shift shift, int target) : base(shift, target, roster.StartDate)
    {
        Roster = roster;
        RosterID = roster.ID;
    }
}
