using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace Uranus.Staff.Model
{
    public class Shift
    {
        [PrimaryKey]
        public string Name { get; set; }
        [ForeignKey(typeof(Department))]
        public string DepartmentName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string BreakString { get; set; }

        [ManyToOne(inverseProperty: "Shifts")]
        public Department Department { get; set; }
        [Ignore]
        public List<Break> Breaks { get; set; }
        [ManyToMany(typeof(EmployeeShift), "EmployeeID", "Shifts", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Employee> Employees { get; set; }

        [OneToMany(nameof(Roster.ShiftName), nameof(Roster.Shift), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
        public List<Roster> Rosters { get; set; }
    }

    public class Break
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public int Length { get; set; } // in minutes

        public Break() { }

        public Break(string name, DateTime startTime, int length)
        {
            Name = name;
            StartTime = startTime;
            Length = length;
        }
    }
}
