﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace Uranus.Staff.Models;

public class EmployeeRoster : IEquatable<EmployeeRoster>, IComparable<EmployeeRoster>
{
    [PrimaryKey] public Guid ID { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    [ForeignKey(typeof(Shift))] public string ShiftID { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }
    [ForeignKey(typeof(DepartmentRoster))] public Guid DepartmentRosterID { get; set; }
    public DateTime StartDate { get; set; }
    public ERosterType RosterType { get; set; }

    [ManyToOne(nameof(DepartmentName), nameof(Models.Department.EmployeeRosters),
        CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }

    [ManyToOne(nameof(ShiftID), nameof(Models.Shift.EmployeeRosters),
        CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Shift? Shift { get; set; }

    [ManyToOne(nameof(EmployeeID), nameof(Models.Employee.EmployeeRosters),
        CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }

    [ManyToOne(nameof(DepartmentRosterID), nameof(Models.DepartmentRoster.EmployeeRosters),
        CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public DepartmentRoster? DepartmentRoster { get; set; }

    [OneToMany(nameof(Roster.EmployeeRosterID), nameof(Roster.EmployeeRoster))]
    public List<Roster> Rosters { get; set; }


    public EmployeeRoster()
    {
        ID = Guid.NewGuid();
        DepartmentName = string.Empty;
        ShiftID = string.Empty;
        Rosters = new List<Roster>();
    }

    public EmployeeRoster(Department department, DepartmentRoster departmentRoster, Employee employee, DateTime startDate)
    {
        ID = Guid.NewGuid();
        Department = department;
        DepartmentName = Department.Name;
        Employee = employee;
        EmployeeID = Employee.ID;
        StartDate = startDate;
        ShiftID = string.Empty;
        DepartmentRoster = departmentRoster;
        DepartmentRosterID = DepartmentRoster.ID;
        Rosters = new List<Roster>();
    }

    public bool Equals(EmployeeRoster? other)
    {
        if (other is null) return false;
        return ID == other.ID || DepartmentRosterID == other.DepartmentRosterID && EmployeeID == other.EmployeeID && StartDate == other.StartDate;
    }

    public int CompareTo(EmployeeRoster? other)
    {
        if (other is null) return 1;
        if (Employee is null) return other.Employee is null ? 0 : -1;
        return string.Compare(Employee.FullName, other.Employee?.FullName ?? "", StringComparison.Ordinal);
    }
}