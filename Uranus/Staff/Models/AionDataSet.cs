using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Models;

/// <summary>
/// A class for holding (converted to current) aion-relevant data.
/// </summary>
public class AionDataSet
{
    public Dictionary<int, Employee> Employees { get; set; }
    public Dictionary<Guid, ClockEvent> ClockEvents { get; set; }
    public Dictionary<Guid, ShiftEntry> ShiftEntries { get; set; }

    public AionDataSet()
    {
        Employees = new Dictionary<int, Employee>();
        ClockEvents = new Dictionary<Guid, ClockEvent>();
        ShiftEntries = new Dictionary<Guid, ShiftEntry>();
    }

    public AionDataSet(Dictionary<int, Employee> employees, Dictionary<Guid, ClockEvent> clockEvents, Dictionary<Guid, ShiftEntry> shiftEntries)
    {
        Employees = employees;
        ClockEvents = clockEvents;
        ShiftEntries = shiftEntries;
    }

    public bool HasData()
    {
        return HasEmployees() || HasClockEvents() || HasShiftEntries();
    }

    public bool HasEmployees()
    {
        return Employees.Any();
    }

    public bool HasClockEvents()
    {
        return ClockEvents.Any();
    }

    public bool HasShiftEntries()
    {
        return ShiftEntries.Any();
    }
}