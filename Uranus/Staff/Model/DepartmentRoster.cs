using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Model;

public class DepartmentRoster
{
    [PrimaryKey] public Guid ID { get; set; }
    public string Name { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    public DateTime StartDate { get; set; } // Monday

    [ManyToOne(nameof(DepartmentName), nameof(Model.Department.DepartmentRosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }

    [OneToMany(nameof(Roster.DepartmentRosterID), nameof(Roster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Roster> Rosters { get; set; }
    [OneToMany(nameof(DailyRoster.DepartmentRosterID), nameof(DailyRoster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<DailyRoster> DailyRosters { get; set; }
    [OneToMany(nameof(EmployeeRoster.DepartmentRosterID), nameof(EmployeeRoster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<EmployeeRoster> EmployeeRosters { get; set; }

    [Ignore] public Dictionary<int, Employee> EmployeeDict { get; set; }
    [Ignore] public Dictionary<string, Shift> ShiftDict { get; set; }
    [Ignore] public Dictionary<string, List<Break>> BreakDict { get; set; }
    [Ignore] public Dictionary<(int, DateTime), Roster> RosterDict { get; set; }
    [Ignore] public Dictionary<Guid, DailyRoster> DailyRosterDict { get; set; }
    [Ignore] public Dictionary<Guid, EmployeeRoster> EmployeeRosterDict { get; set; }
    [Ignore] public IEnumerable<EmployeeShift> EmpShiftConnections { get; set; }
    [Ignore] public Dictionary<int, List<ShiftRule>> ShiftRuleDict { get; set; }

    [Ignore] public Shift? DefaultShift { get; set; }

    public DepartmentRoster()
    {
        Name = string.Empty;
        DepartmentName = string.Empty;
        Rosters = new List<Roster>();
        DailyRosters = new List<DailyRoster>();
        EmployeeRosters = new List<EmployeeRoster>();

        EmployeeDict = new Dictionary<int, Employee>();
        ShiftDict = new Dictionary<string, Shift>();
        BreakDict = new Dictionary<string, List<Break>>();
        RosterDict = new Dictionary<(int, DateTime), Roster>();
        DailyRosterDict = new Dictionary<Guid, DailyRoster>();
        EmployeeRosterDict = new Dictionary<Guid, EmployeeRoster>();
        EmpShiftConnections = new List<EmployeeShift>();
        ShiftRuleDict = new Dictionary<int, List<ShiftRule>>();
    }

    public void SetData(IEnumerable<Employee> employees, IEnumerable<Roster> rosters, IEnumerable<DailyRoster> dailyRosters, IEnumerable<EmployeeRoster> employeeRosters,
        IEnumerable<Shift> shifts, IEnumerable<Break> breaks, IEnumerable<EmployeeShift> empShiftCons, IEnumerable<ShiftRule> shiftRules)
    {
        EmployeeDict = employees.ToDictionary(e => e.ID, e => e);
        ShiftDict = shifts.ToDictionary(s => s.ID, s => s);
        BreakDict = breaks.GroupBy(b => b.ShiftID).ToDictionary(g => g.Key, g => g.ToList());
        EmpShiftConnections = empShiftCons;
        ShiftRuleDict = shiftRules.GroupBy(r => r.EmployeeID).ToDictionary(g => g.Key, g => g.ToList());
        RosterDict = rosters.ToDictionary(r => (r.EmployeeID, r.Date), r => r);
        EmployeeRosterDict = employeeRosters.ToDictionary(er => er.ID, er => er);
        DailyRosterDict = dailyRosters.ToDictionary(dr => dr.ID, dr => dr);

        SetRelationships();
    }

    /// <summary>
    /// Using the dictionaries, assign relevant data as references to each other.
    /// </summary>
    public void SetRelationships()
    {
        SetFromShifts();
        SetFromConnections();
        SetFromEmployees();
        SetFromRosters();
        SetFromDailyRosters();
        SetFromEmployeeRosters();
    }

    /// <summary>
    /// Generates an empty roster for each that should exist based on employees and date range.
    /// </summary>
    private void GenerateMissingRosters()
    {
        // TODO: Generate missing Rosters.
    }

    private void SetFromShifts()
    {
        foreach (var (_, shift) in ShiftDict)
        {
            if (BreakDict.TryGetValue(shift.ID, out var breaks))
            {
                shift.Breaks = breaks;
                foreach (var @break in breaks)
                    @break.Shift = shift;
            }
            Department?.Shifts.Add(shift);
            shift.Department = Department;
            if (shift.Default) DefaultShift = shift;
        }
    }

    private void SetFromConnections()
    {
        foreach (var empShiftConnection in EmpShiftConnections)
        {
            EmployeeDict.TryGetValue(empShiftConnection.EmployeeID, out var employee);
            ShiftDict.TryGetValue(empShiftConnection.ShiftID, out var shift);

            empShiftConnection.Employee = employee;
            empShiftConnection.Shift = shift;

            if (employee is null || shift is null) continue;

            employee.Shifts.Add(shift);
            shift.Employees.Add(employee);
        }
    }

    private void SetFromEmployees()
    {
        foreach (var (_, employee) in EmployeeDict)
        {
            if (employee.DepartmentName == Department?.Name) employee.Department = Department;

            if (DefaultShift is not null) employee.Shifts.Add(DefaultShift);

            if (!ShiftRuleDict.TryGetValue(employee.ID, out var shiftRules)) continue;

            employee.Rules = shiftRules;
            foreach (var shiftRule in shiftRules)
                shiftRule.Employee = employee;
        }
    }

    private void SetFromRosters()
    {
        foreach (var (_, roster) in RosterDict)
        {
            if (roster.DepartmentName == Department?.Name) roster.Department = Department;

            if (EmployeeDict.TryGetValue(roster.EmployeeID, out var employee))
            {
                employee.Rosters.Add(roster);
                roster.Employee = employee;
            }

            if (ShiftDict.TryGetValue(roster.ShiftID, out var shift))
            {
                roster.Shift = shift;
                shift.Rosters.Add(roster);
            }

            if (DailyRosterDict.TryGetValue(roster.DailyRosterID, out var daily))
            {
                roster.DailyRoster = daily;
                daily.Rosters.Add(roster);
            }

            if (EmployeeRosterDict.TryGetValue(roster.EmployeeRosterID, out var employeeRoster))
            {
                roster.EmployeeRoster = employeeRoster;
                employeeRoster.Rosters.Add(roster);
            }

            if (roster.DepartmentRosterID == ID)
            {
                Rosters.Add(roster);
                roster.DepartmentRoster = this;
            }
        }
    }

    private void SetFromDailyRosters()
    {
        foreach (var (_, dailyRoster) in DailyRosterDict)
        {
            if (dailyRoster.DepartmentRosterID == ID)
            {
                dailyRoster.DepartmentRoster = this;
                DailyRosters.Add(dailyRoster);
            }

            if (dailyRoster.DepartmentName == DepartmentName)
            {
                dailyRoster.Department = Department;
                Department?.DailyRosters.Add(dailyRoster);
            }
        }
    }

    private void SetFromEmployeeRosters()
    {
        foreach (var (_, employeeRoster) in EmployeeRosterDict)
        {
            if (employeeRoster.DepartmentName == DepartmentName)
            {
                employeeRoster.Department = Department;
                Department?.EmployeeRosters.Add(employeeRoster);
            }

            if (employeeRoster.DepartmentRosterID == ID)
            {
                employeeRoster.DepartmentRoster = this;
                EmployeeRosters.Add(employeeRoster);
            }

            if (EmployeeDict.TryGetValue(employeeRoster.EmployeeID, out var employee))
            {
                employee.EmployeeRosters.Add(employeeRoster);
                employeeRoster.Employee = employee;
            }

            if (ShiftDict.TryGetValue(employeeRoster.ShiftID, out var shift))
            {
                shift.EmployeeRosters.Add(employeeRoster);
                employeeRoster.Shift = shift;
            }
        }
    }
}