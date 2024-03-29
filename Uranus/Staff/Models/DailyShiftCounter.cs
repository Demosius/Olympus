﻿using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Staff.Models;

public class DailyShiftCounter : ShiftCounter
{
    [ForeignKey(typeof(DailyRoster))] public Guid RosterID { get; set; }

    [ManyToOne(nameof(RosterID), nameof(DailyRoster.ShiftCounters), CascadeOperations = CascadeOperation.None)]
    public DailyRoster? Roster { get; set; }

    public DailyShiftCounter() { }

    public DailyShiftCounter(DailyRoster roster, Shift shift, int target) : base(shift, target)
    {
        Roster = roster;
        RosterID = roster.ID;
    }
}