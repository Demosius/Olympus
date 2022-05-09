using SQLiteNetExtensions.Attributes;
using System;
using System.Linq;
using SQLite;

namespace Uranus.Staff.Model;

public class WeeklyShiftCounter : ShiftCounter
{
    [ForeignKey(typeof(DepartmentRoster))] public Guid RosterID { get; set; }

    [ManyToOne(nameof(RosterID), nameof(DepartmentRoster.ShiftCounters), CascadeOperations = CascadeOperation.None)]
    public DepartmentRoster? Roster { get; set; }

    /// <summary>
    /// Used for handling the targets of daily shifts of the roster as well.
    /// </summary>
    [Ignore] public int MasterTarget
    {
        get => Target;
        set
        {
            if (Roster is null) throw new ArgumentNullException(nameof(Roster));
            
            foreach (var dailyCounter in Roster.DailyRosters
                         .SelectMany(dailyRoster => dailyRoster.ShiftCounters
                             .Where(dailyCounter => dailyCounter.ShiftID == ShiftID)))
            {
                dailyCounter.Target -= Target;
                dailyCounter.Target += value;
                if (dailyCounter.Target < 0) dailyCounter.Target = 0;
            }

            Target = value;
        }
    }

    public WeeklyShiftCounter() { }

    public WeeklyShiftCounter(DepartmentRoster roster, Shift shift, int target) : base(shift, target, roster.StartDate)
    {
        Roster = roster;
        RosterID = roster.ID;
    }
}
