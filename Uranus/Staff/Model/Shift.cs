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

    [ManyToOne(nameof(DepartmentName), nameof(Model.Department.Shifts), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }

    [ManyToMany(typeof(EmployeeShift), nameof(EmployeeShift.ShiftName), nameof(Employee.Shifts), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Employees { get; set; }

    [OneToMany(nameof(Roster.ShiftName), nameof(Roster.Shift), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public List<Roster> Rosters { get; set; }

    [Ignore] public List<Break> Breaks { get; set; }

    public Shift()
    {
        Name = string.Empty;
        DepartmentName = string.Empty;
        BreakString = string.Empty;
        Employees = new List<Employee>();
        Rosters = new List<Roster>();
        Breaks = new List<Break>();
    }

    public Shift(string name, string departmentName, DateTime startTime, DateTime endTime, string breakString, Department department, List<Employee> employees, List<Roster> rosters, List<Break> breaks)
    {
        Name = name;
        DepartmentName = departmentName;
        StartTime = startTime;
        EndTime = endTime;
        BreakString = breakString;
        Department = department;
        Employees = employees;
        Rosters = rosters;
        Breaks = breaks;
    }
}

public class Break : IComparable
{
    public string Name { get; set; }
    public DateTime StartTime { get; set; }
    public int Length { get; set; } // in minutes

    public Break()
    {
        Name = string.Empty;
    }

    public Break(string name, DateTime startTime, int length)
    {
        Name = name;
        StartTime = startTime;
        Length = length;
    }

    public int CompareTo(object? obj)
    {
        if (obj is not Break otherBreak) return -1;
        return StartTime.CompareTo(otherBreak.StartTime);
    }
}