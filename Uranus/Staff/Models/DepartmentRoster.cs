using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Uranus.Staff.Models;

public class DepartmentRoster
{
    [PrimaryKey] public Guid ID { get; set; }
    public string Name { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    public DateTime StartDate { get; set; } // Monday
    public bool UseSaturdays { get; set; }
    public bool UseSundays { get; set; }
    public bool ExceedTargets { get; set; }

    [ManyToOne(nameof(DepartmentName), nameof(Models.Department.DepartmentRosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }

    [OneToMany(nameof(Roster.DepartmentRosterID), nameof(Roster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Roster> Rosters { get; set; }
    [OneToMany(nameof(DailyRoster.DepartmentRosterID), nameof(DailyRoster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<DailyRoster> DailyRosters { get; set; }
    [OneToMany(nameof(EmployeeRoster.DepartmentRosterID), nameof(EmployeeRoster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<EmployeeRoster> EmployeeRosters { get; set; }
    [OneToMany(nameof(WeeklyShiftCounter.RosterID), nameof(WeeklyShiftCounter.Roster), CascadeOperations = CascadeOperation.None)]
    public List<WeeklyShiftCounter> ShiftCounters { get; set; }

    [Ignore] public Dictionary<int, Employee> EmployeeDict { get; set; }
    [Ignore] public Dictionary<string, Shift> ShiftDict { get; set; }
    [Ignore] public Dictionary<string, List<Break>> BreakDict { get; set; }
    [Ignore] public Dictionary<(int, DateTime), Roster> RosterDict { get; set; }
    [Ignore] public Dictionary<Guid, DailyRoster> DailyRosterDict { get; set; }
    [Ignore] public Dictionary<Guid, EmployeeRoster> EmployeeRosterDict { get; set; }
    [Ignore] public IEnumerable<EmployeeShift> EmpShiftConnections { get; set; }
    [Ignore] public Dictionary<int, List<ShiftRuleSingle>> SingleRuleDict { get; set; }
    [Ignore] public Dictionary<int, List<ShiftRuleRecurring>> RecurringRuleDict { get; set; }
    [Ignore] public Dictionary<int, List<ShiftRuleRoster>> RosterRuleDict { get; set; }
    [Ignore] public Dictionary<string, WeeklyShiftCounter> ShiftCounterDict { get; set; }
    [Ignore] public Dictionary<Guid, Dictionary<string, DailyShiftCounter>> DailyCounterDict { get; set; }

    [Ignore] public Shift? DefaultShift { get; set; }

    [Ignore] public Dictionary<Shift, int> ShiftCounter { get; set; }
    [Ignore] public Dictionary<Shift, int> TargetShiftCounts { get; set; }

    [Ignore] public bool IsLoaded { get; set; }

    public DepartmentRoster()
    {
        Name = string.Empty;
        DepartmentName = string.Empty;
        Rosters = new List<Roster>();
        DailyRosters = new List<DailyRoster>();
        EmployeeRosters = new List<EmployeeRoster>();
        ShiftCounters = new List<WeeklyShiftCounter>();

        EmployeeDict = new Dictionary<int, Employee>();
        ShiftDict = new Dictionary<string, Shift>();
        BreakDict = new Dictionary<string, List<Break>>();
        RosterDict = new Dictionary<(int, DateTime), Roster>();
        DailyRosterDict = new Dictionary<Guid, DailyRoster>();
        EmployeeRosterDict = new Dictionary<Guid, EmployeeRoster>();
        EmpShiftConnections = new List<EmployeeShift>();
        SingleRuleDict = new Dictionary<int, List<ShiftRuleSingle>>();
        RecurringRuleDict = new Dictionary<int, List<ShiftRuleRecurring>>();
        RosterRuleDict = new Dictionary<int, List<ShiftRuleRoster>>();
        ShiftCounter = new Dictionary<Shift, int>();
        TargetShiftCounts = new Dictionary<Shift, int>();
        ShiftCounterDict = new Dictionary<string, WeeklyShiftCounter>();
        DailyCounterDict = new Dictionary<Guid, Dictionary<string, DailyShiftCounter>>();
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
        ShiftCounters = new List<WeeklyShiftCounter>();

        EmployeeDict = new Dictionary<int, Employee>();
        ShiftDict = new Dictionary<string, Shift>();
        BreakDict = new Dictionary<string, List<Break>>();
        RosterDict = new Dictionary<(int, DateTime), Roster>();
        DailyRosterDict = new Dictionary<Guid, DailyRoster>();
        EmployeeRosterDict = new Dictionary<Guid, EmployeeRoster>();
        EmpShiftConnections = new List<EmployeeShift>();
        SingleRuleDict = new Dictionary<int, List<ShiftRuleSingle>>();
        RecurringRuleDict = new Dictionary<int, List<ShiftRuleRecurring>>();
        RosterRuleDict = new Dictionary<int, List<ShiftRuleRoster>>();
        ShiftCounter = new Dictionary<Shift, int>();
        TargetShiftCounts = new Dictionary<Shift, int>();
        ShiftCounterDict = new Dictionary<string, WeeklyShiftCounter>();
        DailyCounterDict = new Dictionary<Guid, Dictionary<string, DailyShiftCounter>>();
    }

    public void SetData(IEnumerable<Employee> employees, IEnumerable<Roster> rosters, IEnumerable<DailyRoster> dailyRosters, IEnumerable<EmployeeRoster> employeeRosters,
        IEnumerable<Shift> shifts, IEnumerable<Break> breaks, IEnumerable<EmployeeShift> empShiftCons,
        IEnumerable<ShiftRuleSingle> singleRules, IEnumerable<ShiftRuleRecurring> recurringRules, IEnumerable<ShiftRuleRoster> rosterRules,
        IEnumerable<WeeklyShiftCounter> weeklyShiftCounters, IEnumerable<DailyShiftCounter> dailyShiftCounters)
    {
        EmployeeDict = employees.ToDictionary(e => e.ID, e => e);
        ShiftDict = shifts.ToDictionary(s => s.ID, s => s);
        BreakDict = breaks.GroupBy(b => b.ShiftID).ToDictionary(g => g.Key, g => g.ToList());
        EmpShiftConnections = empShiftCons;
        SingleRuleDict = singleRules.GroupBy(rule => rule.EmployeeID)
            .ToDictionary(group => group.Key, group => group.ToList());
        RecurringRuleDict = recurringRules.GroupBy(rule => rule.EmployeeID)
            .ToDictionary(group => group.Key, group => group.ToList());
        RosterRuleDict = rosterRules.GroupBy(rule => rule.EmployeeID)
            .ToDictionary(group => group.Key, group => group.ToList());
        RosterDict = rosters.ToDictionary(r => (r.EmployeeID, r.Date), r => r);
        EmployeeRosterDict = employeeRosters.ToDictionary(er => er.ID, er => er);
        DailyRosterDict = dailyRosters.ToDictionary(dr => dr.ID, dr => dr);

        DailyCounterDict = dailyShiftCounters.GroupBy(counter => counter.RosterID).ToDictionary(group => group.Key,
            group => group.ToDictionary(counter => counter.ShiftID, counter => counter));

        ShiftCounterDict = weeklyShiftCounters.Where(c => c.RosterID == ID).ToDictionary(c => c.ShiftID, c => c);
        ShiftCounters = ShiftCounterDict.Values.ToList();

        SetRelationships();
        GenerateMissingRosters();
        EmployeeRosters.Sort();
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
                dailyRoster.AddRoster(roster);
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

            if (ShiftCounterDict.TryGetValue(shift.ID, out var counter))
            {
                counter.Shift = shift;
                if (counter.RosterID == ID) counter.Roster = this;
            }
            else
            {
                counter = new WeeklyShiftCounter(this, shift, shift.DailyTarget);
                ShiftCounterDict.Add(shift.ID, counter);
                ShiftCounters.Add(counter);
            }

            if (Department is not null && !Department.Shifts.Select(s => s.Name).Contains(shift.Name))
                Department?.Shifts.Add(shift);

            ShiftCounter[shift] = 0;
            TargetShiftCounts[shift] = shift.DailyTarget;
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

            if (shift != DefaultShift) employee.Shifts.Add(shift);
            shift.Employees.Add(employee);
        }
    }

    private void SetFromEmployees()
    {
        foreach (var (_, employee) in EmployeeDict)
        {
            if (employee.DepartmentName == Department?.Name) employee.Department = Department;

            if (DefaultShift is not null) employee.Shifts.Add(DefaultShift);

            if (SingleRuleDict.TryGetValue(employee.ID, out var shiftRules))
            {
                employee.SingleRules = shiftRules;
                foreach (var shiftRule in shiftRules)
                    shiftRule.Employee = employee;
            }

            if (RecurringRuleDict.TryGetValue(employee.ID, out var recurringRules))
            {
                employee.RecurringRules = recurringRules;
                foreach (var shiftRule in recurringRules)
                    shiftRule.Employee = employee;
            }

            if (!RosterRuleDict.TryGetValue(employee.ID, out var rosterRules)) continue;

            employee.RosterRules = rosterRules;
            foreach (var shiftRule in rosterRules)
            {
                shiftRule.Employee = employee;
                if (!ShiftDict.TryGetValue(shiftRule.ShiftID ?? "", out var shift)) continue;

                shiftRule.Shift = shift;
                shift.RosterRules.Add(shiftRule);
            }
        }
    }

    private void SetFromRosters()
    {
        var nullIDs = new List<(int, DateTime)>();

        foreach (var (key, roster) in RosterDict)
        {
            if (roster.DepartmentName == Department?.Name) roster.Department = Department;

            if (EmployeeDict.TryGetValue(roster.EmployeeID, out var employee))
            {
                employee.Rosters.Add(roster);
                roster.Employee = employee;
            }
            else    // Make sure to remove rosters that are trying to use non-existent/inactive employees.
            {
                nullIDs.Add(key);
                continue;
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
                if (roster.Shift is not null && ShiftCounter.TryGetValue(roster.Shift, out _))
                    ShiftCounter[roster.Shift]++;
            }
        }

        foreach (var rosterKey in nullIDs)
        {
            RosterDict.Remove(rosterKey);
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

            DailyCounterDict.TryGetValue(dailyRoster.ID, out var counters);
            counters ??= new Dictionary<string, DailyShiftCounter>();

            foreach (var (id, shift) in ShiftDict)
            {
                counters.TryGetValue(id, out var counter);
                counter ??= new DailyShiftCounter(dailyRoster, shift, ShiftCounterDict[id].Target);

                counter.Shift = shift;
                counter.Roster = dailyRoster;
                dailyRoster.ShiftCounters.Add(counter);
            }
        }
    }

    private void SetFromEmployeeRosters()
    {
        var nullIDs = new List<Guid>();
        foreach (var (_, employeeRoster) in EmployeeRosterDict)
        {
            if (EmployeeDict.TryGetValue(employeeRoster.EmployeeID, out var employee))
            {
                employee.EmployeeRosters.Add(employeeRoster);
                employeeRoster.Employee = employee;
            }
            else
            {
                nullIDs.Add(employeeRoster.ID);
                continue;
            }

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

            if (ShiftDict.TryGetValue(employeeRoster.ShiftID, out var shift))
            {
                shift.EmployeeRosters.Add(employeeRoster);
                employeeRoster.Shift = shift;
            }
        }

        foreach (var id in nullIDs)
        {
            EmployeeRosterDict.Remove(id);
        }
    }

    /// <summary>
    /// Converts the contained array of rosters into a DataTable.
    /// </summary>
    /// <returns></returns>
    public DataTable DataTable()
    {
        var table = new DataTable();

        // Establish columns.
        table.Columns.Add("NAME");
        foreach (var dailyRoster in DailyRosters)
            table.Columns.Add(dailyRoster.Date.ToString("dd-MMM"));

        // Day of week row.
        var row = table.NewRow();
        var i = 0;
        foreach (var dailyRoster in DailyRosters)
        {
            i++;
            row[i] = dailyRoster.Date.ToString("dddd").ToUpper();
        }

        table.Rows.Add(row);

        // Fill in employees.
        foreach (var employeeRoster in EmployeeRosters)
        {
            row = table.NewRow();
            i = 0;
            row[i] = employeeRoster.Employee?.FullName ?? "";
            foreach (var roster in employeeRoster.Rosters)
            {
                i++;
                row[i] = roster.ToString();
            }

            table.Rows.Add(row);
        }

        return table;
    }

    public override string ToString() => Name;
}