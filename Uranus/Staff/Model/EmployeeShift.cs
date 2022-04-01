using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model;

// Denotes an employees eligibility for a particular shift.
public class EmployeeShift : IComparable
{
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }
    [ForeignKey(typeof(Shift))] public string ShiftID { get; set; }

    [Ignore] public Employee? Employee { get; set; }
    [Ignore] public Shift? Shift { get; set; }
    [Ignore] public bool Active { get; set; }
    [Ignore] public bool Original { get; set; }

    public EmployeeShift()
    {
        ShiftID = string.Empty;
    }

    public EmployeeShift(int employeeID, string shiftID)
    {
        EmployeeID = employeeID;
        ShiftID = shiftID;
    }

    public EmployeeShift(Employee employee, Shift shift)
    {
        Employee = employee;
        Shift = shift;
        EmployeeID = employee.ID;
        ShiftID = shift.ID;
    }

    public int CompareTo(object? obj)
    {
        if (obj is not EmployeeShift other) return -1;
        if (Employee is not null && other.Employee is not null)
            return string.Compare(Employee.FullName, other.Employee.FullName, StringComparison.Ordinal);
        return EmployeeID > other.EmployeeID ? 1 : EmployeeID < other.EmployeeID ? -1 : 0;
    }
}