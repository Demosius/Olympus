using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Staff.Model;

public enum ERosterType
{
    Standard,
    AL,
    PCL,
    RDO,
    PublicHoliday
}

public class Roster
{
    [PrimaryKey] public Guid ID { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }
    [ForeignKey(typeof(Shift))] public string ShiftID { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    [ForeignKey(typeof(DailyRoster))] public Guid DailyRosterID { get; set; }
    [ForeignKey(typeof(EmployeeRoster))] public Guid EmployeeRosterID { get; set; }
    [ForeignKey(typeof(DepartmentRoster))] public Guid DepartmentRosterID { get; set; }
    public DayOfWeek Day { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public ERosterType RosterType { get; set; }

    [ManyToOne(nameof(EmployeeID), nameof(Model.Employee.Rosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }
    [ManyToOne(nameof(ShiftID), nameof(Model.Shift.Rosters), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public Shift? Shift { get; set; }
    [ManyToOne(nameof(DepartmentName), nameof(Model.Department.Rosters), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public Department? Department { get; set; }
    [ManyToOne(nameof(DailyRosterID), nameof(Model.DailyRoster.Rosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public DailyRoster? DailyRoster { get; set; }
    [ManyToOne(nameof(EmployeeRosterID), nameof(Model.EmployeeRoster.Rosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public EmployeeRoster? EmployeeRoster { get; set; }
    [ManyToOne(nameof(DepartmentRosterID), nameof(Model.DepartmentRoster.Rosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public DepartmentRoster? DepartmentRoster { get; set; }


    public Roster()
    {
        ID = Guid.NewGuid();
        ShiftID = string.Empty;
        DepartmentName = string.Empty;
    }

    public Roster(Employee employee, DateTime date)
    {
        ID = Guid.NewGuid();
        EmployeeID = employee.ID;
        Employee = employee;
        ShiftID = string.Empty;

        if (Employee.Department is not null)
        {
            Department = Employee.Department;
            DepartmentName = Employee.DepartmentName;
        }
        else
        {
            DepartmentName = employee.DepartmentName;
        }

        Date = date;
        Day = Date.DayOfWeek;
    }

    public Roster(Department department, Employee employee, DateTime date)
    {
        ID = Guid.NewGuid();
        EmployeeID = employee.ID;
        Employee = employee;
        ShiftID = string.Empty;

        Department = department;
        DepartmentName = department.Name;

        Date = date;
        Day = Date.DayOfWeek;
    }

    public Roster(Guid id, int employeeID, string shiftID, string departmentName, DayOfWeek day, DateTime date, ERosterType rosterType, Employee employee, Shift shift, Department department)
    {
        ID = id;
        EmployeeID = employeeID;
        ShiftID = shiftID;
        DepartmentName = departmentName;
        Day = day;
        Date = date;
        RosterType = rosterType;
        Employee = employee;
        Shift = shift;
        Department = department;
    }

    public void SetShift(Shift newShift)
    {
        Shift = newShift;
        ShiftID = newShift.ID;
        StartTime = newShift.StartTime;
        EndTime = newShift.EndTime;
    }

    public void SetDate(DateTime date)
    {
        Date = date;
        Day = Date.DayOfWeek;
    }
}