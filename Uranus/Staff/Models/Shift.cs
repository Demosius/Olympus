using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Models;

public class Shift : IEquatable<Shift>, IComparable<Shift>
{
    [PrimaryKey] public string ID { get; set; } // {DepartmentName}|{Name}
    public string Name { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int DailyTarget { get; set; }
    
    public bool Default { get; set; }

    [ManyToOne(nameof(DepartmentName), nameof(Models.Department.Shifts), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }

    [ManyToMany(typeof(EmployeeShift), nameof(EmployeeShift.ShiftID), nameof(Employee.Shifts), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Employees { get; set; }

    [OneToMany(nameof(Roster.ShiftID), nameof(Roster.Shift), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public List<Roster> Rosters { get; set; }
    [OneToMany(nameof(EmployeeRoster.ShiftID), nameof(EmployeeRoster.Shift), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<EmployeeRoster> EmployeeRosters { get; set; }
    [OneToMany(nameof(Break.ShiftID), nameof(Break.Shift), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public List<Break> Breaks { get; set; }
    [OneToMany(nameof(Employee.DefaultShiftID), nameof(Employee.DefaultShift), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> DefaultEmployees { get; set; }
    [OneToMany(nameof(ShiftRuleRoster.ShiftID), nameof(ShiftRuleRoster.Shift), CascadeOperations = CascadeOperation.None)]
    public List<ShiftRuleRoster> RosterRules { get; set; }

    public Shift()
    {
        Name = string.Empty;
        DepartmentName = string.Empty;
        Employees = new List<Employee>();
        Rosters = new List<Roster>();
        EmployeeRosters = new List<EmployeeRoster>();
        Breaks = new List<Break>();
        DefaultEmployees = new List<Employee>();
        RosterRules = new List<ShiftRuleRoster>();
        ID = string.Empty;
    }

    public Shift(Department department, string name)
    {
        Name = name;
        DepartmentName = department.Name;
        Department = department;
        ID = $"{DepartmentName}|{Name}";
        Employees = new List<Employee>();
        Rosters = new List<Roster>();
        EmployeeRosters = new List<EmployeeRoster>();
        RosterRules = new List<ShiftRuleRoster>();

        // Select initial times.
        StartTime = TimeSpan.FromHours(8);
        EndTime = TimeSpan.FromHours(16);

        Breaks = new List<Break>
        {
            new (this)
        };
        DefaultEmployees = new List<Employee>();
    }

    public Shift(string name, string departmentName, TimeSpan startTime, TimeSpan endTime, Department department, List<Employee> employees, List<Roster> rosters, List<Break> breaks)
    {
        Name = name;
        DepartmentName = departmentName;
        StartTime = startTime;
        EndTime = endTime;
        Department = department;
        Employees = employees;
        Rosters = rosters;
        EmployeeRosters = new List<EmployeeRoster>();
        Breaks = breaks;
        DefaultEmployees = new List<Employee>();
        RosterRules = new List<ShiftRuleRoster>();
        ID = $"{DepartmentName}|{Name}";
    }

    public void SetBreaks(IEnumerable<Break> newBreaks)
    {
        Breaks = newBreaks.ToList();
        Breaks.Sort();
    }

    public bool Equals(Shift? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ID == other.ID;
    }

    public int CompareTo(Shift? other)
    {
        return other == null ? 1 : string.CompareOrdinal(ID, other.ID);
    }

    public override string ToString() => Name;
}
