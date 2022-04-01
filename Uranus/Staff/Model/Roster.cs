using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Staff.Model;

public enum ERosterType
{
    Standard,
    AL,
    PCL,
    RDO
}

public class Roster
{
    [PrimaryKey] public Guid ID { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }
    [ForeignKey(typeof(Shift))] public string ShiftName { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    public DayOfWeek Day { get; set; }
    public DateTime Date { get; set; }
    public ERosterType RosterType { get; set; }

    [ManyToOne(nameof(EmployeeID), nameof(Model.Employee.Rosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }
    [ManyToOne(nameof(ShiftName), nameof(Model.Shift.Rosters), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public Shift? Shift { get; set; }
    [ManyToOne(nameof(DepartmentName), nameof(Model.Department.Rosters), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public Department? Department { get; set; }

    public Roster()
    {
        ID = Guid.NewGuid();
        ShiftName = string.Empty;
        DepartmentName = string.Empty;
    }

    public Roster(Guid id, int employeeID, string shiftName, string departmentName, DayOfWeek day, DateTime date, ERosterType rosterType, Employee employee, Shift shift, Department department)
    {
        ID = id;
        EmployeeID = employeeID;
        ShiftName = shiftName;
        DepartmentName = departmentName;
        Day = day;
        Date = date;
        RosterType = rosterType;
        Employee = employee;
        Shift = shift;
        Department = department;
    }
}