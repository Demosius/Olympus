using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace Uranus.Staff.Model;

public class DailyRoster
{
    [PrimaryKey] public Guid ID { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    [ForeignKey(typeof(DepartmentRoster))] public Guid DepartmentRosterID { get; set; }
    public DateTime Date { get; set; }
    public DayOfWeek Day { get; set; }

    [ManyToOne(nameof(DepartmentName), nameof(Model.Department.DailyRosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }
    [ManyToOne(nameof(DepartmentRosterID), nameof(Model.DepartmentRoster.DailyRosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public DepartmentRoster? DepartmentRoster { get; set; }

    [OneToMany(nameof(Roster.DailyRosterID), nameof(Roster.DailyRoster), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Roster> Rosters { get; set; }

    // TODO: Add shift counting.

    public DailyRoster()
    {
        DepartmentName = string.Empty;
        Rosters = new List<Roster>();
    }
}