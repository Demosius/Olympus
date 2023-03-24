using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Models;

/// <summary>
/// The roster for a department for a single day.
/// Will reference the Department Roster object, as well as holding a list of basic Roster objects.
/// </summary>
public class DailyRoster : IEquatable<DailyRoster>, IComparable<DailyRoster>
{
    [PrimaryKey] public Guid ID { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    [ForeignKey(typeof(DepartmentRoster))] public Guid DepartmentRosterID { get; set; }
    public DateTime Date { get; set; }
    public DayOfWeek Day { get; set; }
    public bool IsPublicHoliday { get; set; }

    [ManyToOne(nameof(DepartmentName), nameof(Models.Department.DailyRosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }

    [OneToMany(nameof(Roster.DailyRosterID), nameof(Roster.DailyRoster), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Roster> Rosters { get; set; }
    [OneToMany(nameof(DailyShiftCounter.RosterID), nameof(DailyShiftCounter.Roster), CascadeOperations = CascadeOperation.None)]
    public List<DailyShiftCounter> ShiftCounters { get; set; }

    [OneToOne(nameof(DepartmentRosterID), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public DepartmentRoster? DepartmentRoster { get; set; }
    
    [Ignore] public List<Shift> Shifts => Department?.Shifts ?? new List<Shift>();

    public DailyRoster()
    {
        DepartmentName = string.Empty;
        Rosters = new List<Roster>();
        ShiftCounters = new List<DailyShiftCounter>();
    }

    public DailyRoster(Department department, DepartmentRoster departmentRoster, DateTime date)
    {
        ID = Guid.NewGuid();
        Department = department;
        DepartmentName = Department.Name;
        DepartmentRoster = departmentRoster;
        DepartmentRosterID = DepartmentRoster.ID;
        Date = date;
        Day = date.DayOfWeek;
        Rosters = new List<Roster>();
        // Create appropriate shift counters if possible.
        ShiftCounters = department.Shifts.Select(s => new DailyShiftCounter(this, s, s.DailyTarget)).ToList();
    }

    public DailyShiftCounter ShiftCounter(Shift shift)
    {
        var counter = ShiftCounters.FirstOrDefault(c => c.ShiftID == shift.ID);

        if (counter is not null) return counter;

        counter = new DailyShiftCounter(this, shift, 0);
        ShiftCounters.Add(counter);

        return counter;
    }
    
    public bool Equals(DailyRoster? other)
    {
        if (other is null) return false;
        return ID == other.ID || DepartmentRosterID == other.DepartmentRosterID && Date == other.Date;
    }

    public int CompareTo(DailyRoster? other)
    {
        return other is null ? 1 : Date.CompareTo(other.Date);
    }

    /// <summary>
    ///  Adds the given roster to the daily roster list and applies appropriate other functionality such as shift count.
    /// </summary>
    /// <param name="roster"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void AddRoster(Roster roster)
    {
        Rosters.Add(roster);
        roster.DailyRoster = this;
        roster.DailyRosterID = ID;
    }
    
    public override string ToString() => $"{Day} - {Date:dd MMM yyyy}";
}