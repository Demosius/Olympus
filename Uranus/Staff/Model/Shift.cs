using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace Uranus.Staff.Model;

public class Shift
{
    [PrimaryKey] public string Name { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string BreakString { get; set; }

    [ManyToOne( nameof(DepartmentName), nameof(Model.Department.Shifts), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department Department { get; set; }

    [ManyToMany(typeof(EmployeeShift), nameof(EmployeeShift.ShiftName), nameof(Employee.Shifts), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Employees { get; set; }

    [OneToMany(nameof(Roster.ShiftName), nameof(Roster.Shift), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public List<Roster> Rosters { get; set; }

    [Ignore] public List<Break> Breaks { get; set; }
}

public class Break : IComparable
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

    public int CompareTo(object obj)
    {
        if (obj is not Break otherBreak) return -1;
        return StartTime.CompareTo(otherBreak.StartTime);
    }
}