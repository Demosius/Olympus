using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Uranus.Extension;

namespace Uranus.Staff.Model;

public class DepartmentRoster
{
    [PrimaryKey] public Guid ID { get; set; }
    public string Name { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    public DateTime StartDate { get; set; } // Monday
    public bool UseSaturdays { get; set; }
    public bool UseSundays { get; set; }

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

    [Ignore] public Dictionary<string, int> ShiftCounter { get; set; }
    [Ignore] public Dictionary<string, int> TargetShiftCounts { get; set; }

    [Ignore] public bool IsLoaded { get; set; }

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
        ShiftCounter = new Dictionary<string, int>();
        TargetShiftCounts = new Dictionary<string, int>();
    }

    public DepartmentRoster(string name, DateTime startDate, bool useSaturdays, bool useSundays, Department department)
    {
        ID = Guid.NewGuid();
        Name = name;
        StartDate = startDate;
        UseSaturdays = useSaturdays;
        UseSundays = useSundays;
        Department = department;
        DepartmentName = department.Name;


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
        ShiftCounter = new Dictionary<string, int>();
        TargetShiftCounts = new Dictionary<string, int>();
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
        GenerateMissingRosters();
        IsLoaded = true;
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
        if (Department is null) throw new DataException("Department Roster initialized without Department.");

        // Make sure we have a daily roster for each day from start date.
        var dailyRosterDict = DailyRosters.ToDictionary(dr => dr.Date, dr => dr);
        for (var i = 0; i < 7; i++)
        {
            var date = StartDate.AddDays(i);
            if (dailyRosterDict.TryGetValue(date, out _)) continue;

            var newDailyRoster = new DailyRoster(Department, this, date);
            DailyRosters.Add(newDailyRoster);
            DailyRosterDict.Add(newDailyRoster.ID, newDailyRoster);
            dailyRosterDict.Add(date, newDailyRoster);
        }
        DailyRosters.Sort();
        if (DailyRosters.Count != 7)
            throw new DataException(
                "Department Roster has somehow generated a number of daily rosters not equal to 7.");

        // Convert EmpRoster list to dictionary to compare to our employee list (which should be all required for the department).
        var empRosterDict = EmployeeRosters.ToDictionary(er => er.EmployeeID, er => er);

        // Create employeeRoster for each employee - only for those that do not already exist.
        foreach (var (_, employee) in EmployeeDict)
        {
            if (empRosterDict.TryGetValue(employee.ID, out _)) continue;

            EmployeeRoster newEmployeeRoster = new(Department, this, employee, StartDate);

            // Get Roster for each day.
            for (var i = 0; i < 7; i++)
            {
                var date = StartDate.AddDays(i);
                var dailyRoster = dailyRosterDict[date];
                var roster = new Roster(Department, this, newEmployeeRoster, employee, date);
                newEmployeeRoster.Rosters.Add(roster);
                dailyRoster.Rosters.Add(roster);
                Rosters.Add(roster);
                RosterDict.Add((roster.EmployeeID, date), roster);
            }

            newEmployeeRoster.Rosters.Sort();
            EmployeeRosters.Add(newEmployeeRoster);
            EmployeeRosterDict.Add(newEmployeeRoster.ID, newEmployeeRoster);
        }

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
            ShiftCounter[shift.ID] = 0;
            TargetShiftCounts[shift.ID] = shift.DailyTarget;
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
                daily.AddRoster(roster);
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
                if (ShiftCounter.TryGetValue(roster.ShiftID, out _))
                    ShiftCounter[roster.ShiftID]++;
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

    /// <summary>
    /// Use to automate shift assignment.
    /// </summary>
    public void GenerateRosterAssignments()
    {
        AssignDefaults();
        CountToTargets();
        ApplyDepartmentDefault();
    }

    /// <summary>
    /// Assigns shifts to employees based on their defined defaults - only if they do not already have assigned shifts..
    /// </summary>
    public void AssignDefaults()
    {
        foreach (var employeeRoster in EmployeeRosters.Where(employeeRoster => employeeRoster.Shift is null))
            employeeRoster.SetDefault();
    }

    public void DropCount(string shiftID)
    {
        if (ShiftCounter.TryGetValue(shiftID, out _))
            ShiftCounter[shiftID]--;
    }

    public void AddCount(string shiftID)
    {
        if (ShiftCounter.TryGetValue(shiftID, out _))
            ShiftCounter[shiftID]++;
    }

    /// <summary>
    /// Attempt to reach the targeted number for each shift.
    /// </summary>
    public void CountToTargets()
    {
        if (Department is null) throw new DataException("Department Roster has null value for department.");

        // Get every non-default shift with a target above 0.
        var shifts = Department.Shifts.Where(s => !s.Default && s.DailyTarget > 0)
            .ToDictionary(s => s, _ => new List<EmployeeRoster>());

        foreach (var (shift, _) in shifts)
            shifts[shift] = EmployeeRosters.Where(er => er.Employee!.Shifts.Contains(shift)).ToList();

        // Order by most needed (discrepancy between those available and number required to reach target.
        shifts = shifts.OrderBy(s => s.Value.Count - (TargetShiftCounts[s.Key.ID] - ShiftCounter[s.Key.ID]))
            .ToDictionary(e => e.Key, e => e.Value);

        foreach (var (shift, empRosters) in shifts)
        {
            if (TargetShiftCounts[shift.ID] - ShiftCounter[shift.ID] <= 0) continue;
            // Randomize employees #TODO: Check against history to rotate through staff (instead of randomizing).
            empRosters.Shuffle();

            foreach (var employeeRoster in empRosters.Where(employeeRoster => employeeRoster.Shift is null).TakeWhile(_ => ShiftCounter[shift.ID] < TargetShiftCounts[shift.ID]))
            {
                employeeRoster.SetShift(shift);
            }
        }
    }

    /// <summary>
    /// Checks all employees to see if they are assigned. If they aren't, use department defaults if they exist,
    /// otherwise assign a shift that they are eligible for that is closest to its target count.
    /// </summary>
    public void ApplyDepartmentDefault()
    {
        foreach (var employeeRoster in EmployeeRosters.Where(employeeRoster => employeeRoster.Shift is null))
        {
            if (DefaultShift is not null)
            {
                employeeRoster.SetShift(DefaultShift);
                continue;
            }

            if (employeeRoster.Employee is null)
                throw new DataException("Employee roster does not have employee initialized.");

            Shift? bestShift = null;
            var bestCount = int.MinValue;
            foreach (var shift in employeeRoster.Employee.Shifts)
            {
                ShiftCounter.TryGetValue(shift.ID, out var count);
                TargetShiftCounts.TryGetValue(shift.ID, out var target);

                var disc = target - count;

                if (disc <= bestCount) continue;

                bestCount = target - count;
                bestShift = shift;
            }

            if (bestShift is not null) employeeRoster.SetShift(bestShift);
        }
    }
}