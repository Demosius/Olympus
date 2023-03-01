using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Models;

public enum ERosterType
{
    Standard,
    AL,
    PCL,
    RDO,
    PublicHoliday
}

/// <summary>
/// Instance of a single employee's roster for a single day.
/// Should reference the Daily Roster object and the Employee Roster object. Will also reference the Department Roster.
/// </summary>
public class Roster : IEquatable<Roster>, IComparable<Roster>
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

    public bool AtWork { get; set; }

    [ManyToOne(nameof(EmployeeID), nameof(Models.Employee.Rosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }
    [ManyToOne(nameof(ShiftID), nameof(Models.Shift.Rosters), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public Shift? Shift { get; set; }
    [ManyToOne(nameof(DepartmentName), nameof(Models.Department.Rosters), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public Department? Department { get; set; }
    [ManyToOne(nameof(DailyRosterID), nameof(Models.DailyRoster.Rosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public DailyRoster? DailyRoster { get; set; }
    [ManyToOne(nameof(DepartmentRosterID), nameof(Models.DepartmentRoster.Rosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public DepartmentRoster? DepartmentRoster { get; set; }

    [OneToOne(nameof(EmployeeRosterID), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public EmployeeRoster? EmployeeRoster { get; set; }

    [Ignore] public List<Shift> Shifts => EmployeeRoster?.Shifts ?? new List<Shift>();
    [Ignore] public List<ShiftRule> ShiftRules { get; set; }

    public Roster()
    {
        ID = Guid.NewGuid();
        ShiftID = string.Empty;
        DepartmentName = string.Empty;
        AtWork = true;
        ShiftRules = new List<ShiftRule>();
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
        AtWork = true;
        ShiftRules = new List<ShiftRule>();
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
        AtWork = true;
        ShiftRules = new List<ShiftRule>();
    }

    public Roster(Department department, DepartmentRoster departmentRoster, EmployeeRoster employeeRoster, Employee employee, DateTime date)
    {
        ID = Guid.NewGuid();
        Department = department;
        DepartmentName = Department.Name;
        DepartmentRoster = departmentRoster;
        DepartmentRosterID = DepartmentRoster.ID;
        EmployeeRoster = employeeRoster;
        EmployeeRosterID = EmployeeRoster.ID;
        Employee = employee;
        EmployeeID = Employee.ID;
        Date = date;
        Day = Date.DayOfWeek;
        ShiftID = string.Empty;
        AtWork = Day != DayOfWeek.Saturday && Day != DayOfWeek.Sunday;
        ShiftRules = new List<ShiftRule>();
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
        AtWork = true;
        ShiftRules = new List<ShiftRule>();
    }

    public void SetShift(Shift newShift, bool working = true)
    {
        Shift = newShift;
        ShiftID = newShift.ID;
        StartTime = newShift.StartTime;
        EndTime = newShift.EndTime;

        AtWork = working;
    }

    public void SetDate(DateTime date)
    {
        Date = date;
        Day = Date.DayOfWeek;
    }

    public string TimeString()
    {
        return $"{StartTime.Hours:00}:{StartTime.Minutes:00} - {EndTime.Hours:00}:{EndTime.Minutes:00}";
    }

    public void ApplyShiftRules(List<ShiftRule> rules)
    {
        ShiftRules.AddRange(rules.Where(rule => rule.AppliesToDay(Date)));
        ShiftRules = ShiftRules.Distinct().ToList();
    }

    public void SubCount(Shift shift) => DailyRoster?.SubCount(shift);

    public void AddCount(Shift shift) => DailyRoster?.AddCount(shift);

    /// <summary>
    /// Sets the roster type as public holiday without using the Type Setter - which would result in recursive prompting.
    /// </summary>
    public void SetPublicHoliday(bool isPublicHoliday = true)
    {
        RosterType = isPublicHoliday ? ERosterType.PublicHoliday : ERosterType.Standard;
        //OnPropertyChanged(nameof(Type));
        if (isPublicHoliday)
            AtWork = false;
        else
            AtWork = DailyRoster?.Day is not (DayOfWeek.Saturday or DayOfWeek.Sunday) || Shift is not null;
    }

    public override string ToString()
    {
        return AtWork ? TimeString() : (RosterType == ERosterType.Standard ? ERosterType.RDO : RosterType).ToString();
    }

    public bool Equals(Roster? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ID == other.ID;
    }

    public int CompareTo(Roster? other)
    {
        return other is null ? 1 : Date.CompareTo(other.Date);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj switch
        {
            null => false,
            Roster roster => Equals(roster),
            _ => false
        };
    }

    public override int GetHashCode()
    {
        // ReSharper disable NonReadonlyMemberInGetHashCode
        var hashCode = new HashCode();
        hashCode.Add(EmployeeID);
        hashCode.Add(ShiftID);
        hashCode.Add(DepartmentName);
        hashCode.Add(DepartmentRosterID);
        hashCode.Add((int) Day);
        hashCode.Add(Date);
        hashCode.Add(StartTime);
        hashCode.Add(EndTime);
        hashCode.Add((int) RosterType);
        hashCode.Add(AtWork);
        hashCode.Add(Shift);
        hashCode.Add(ShiftRules);
        return hashCode.ToHashCode();
        // ReSharper restore NonReadonlyMemberInGetHashCode
    }


    public static bool operator ==(Roster? left, Roster? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    public static bool operator !=(Roster? left, Roster? right)
    {
        return !(left == right);
    }

    public static bool operator <(Roster? left, Roster? right)
    {
        return left is null ? right is not null : left.CompareTo(right) < 0;
    }

    public static bool operator <=(Roster? left, Roster? right)
    {
        return left is null || left.CompareTo(right) <= 0;
    }

    public static bool operator >(Roster? left, Roster? right)
    {
        return left is not null && left.CompareTo(right) > 0;
    }

    public static bool operator >=(Roster? left, Roster? right)
    {
        return left is null ? right is null : left.CompareTo(right) >= 0;
    }
}