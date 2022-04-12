using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace Uranus.Staff.Model;

public class EmployeeRoster : IEquatable<EmployeeRoster>, IComparable<EmployeeRoster>
{
    [PrimaryKey] public Guid ID { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    [ForeignKey(typeof(Shift))] public string ShiftID { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }
    [ForeignKey(typeof(DepartmentRoster))] public Guid DepartmentRosterID { get; set; }
    public DateTime StartDate { get; set; }

    [ManyToOne(nameof(DepartmentName), nameof(Model.Department.EmployeeRosters),
        CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }

    [ManyToOne(nameof(ShiftID), nameof(Model.Shift.EmployeeRosters),
        CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Shift? Shift { get; set; }

    [ManyToOne(nameof(EmployeeID), nameof(Model.Employee.EmployeeRosters),
        CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }

    [ManyToOne(nameof(DepartmentRosterID), nameof(Model.DepartmentRoster.EmployeeRosters),
        CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public DepartmentRoster? DepartmentRoster { get; set; }

    [OneToMany(nameof(Roster.EmployeeRosterID), nameof(Roster.EmployeeRoster))]
    public List<Roster> Rosters { get; set; }

    // TODO: Add ordered/day specific Roster references.
    // Ordered would likely be easiest/lightest - but might not have required checks and balances.
    // Checks should be done in initial creation, as opposed to when they are being pulled from the DB.
    // Checks only if size is not accurate?

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
        Rosters = new List<Roster>();
    }

    /// <summary>
    /// Sets teh shift to the employee's default if they have one.
    /// </summary>
    public void SetDefault(bool saturdays = false, bool sundays = false)
    {
        var shift = Employee?.DefaultShift;
        if (shift is null) return;

        SetShift(shift, saturdays, sundays);
    }

    public void SetShift(Shift shift, bool saturdays = false, bool sundays = false)
    {
        if (Shift is not null) DepartmentRoster?.DropCount(Shift);
        Shift = shift;
        ShiftID = Shift.ID;
        DepartmentRoster?.AddCount(Shift);

        foreach (var roster in Rosters)
        {
            if (roster.Day == DayOfWeek.Saturday && !saturdays || roster.Day == DayOfWeek.Sunday && !sundays)
                roster.AtWork = false;
            else 
                roster.SetShift(shift);
        }
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