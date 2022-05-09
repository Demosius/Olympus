using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Model;

public class RosterDataSet
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Department? Department { get; set; }
    public Shift? DefaultShift { get; set; }

    public Dictionary<int, Employee> Employees { get; set; }
    public Dictionary<string, Shift> Shifts { get; set; }
    public Dictionary<string, List<Break>> BreakDict { get; set; }
    public Dictionary<(int, DateTime), Roster> Rosters { get; set; }
    public IEnumerable<EmployeeShift> EmpShiftConnections { get; set; }
    public Dictionary<int, List<ShiftRuleSingle>> SingleRuleDict { get; set; }
    public Dictionary<int, List<ShiftRuleRecurring>> RecurringRuleDict { get; set; }
    public Dictionary<int, List<ShiftRuleRoster>> RosterRuleDict { get; set; }
    public Dictionary<Guid, List<DailyShiftCounter>> DailyShiftCounters { get; set; }
    public Dictionary<Guid, List<WeeklyShiftCounter>> WeeklyShiftCounters { get; set; }

    public List<EmployeeRoster> EmployeeRosters { get; set; }
    public List<DailyRoster> DailyRosters { get; set; }

    public RosterDataSet()
    {
        Employees = new Dictionary<int, Employee>();
        Shifts = new Dictionary<string, Shift>();
        BreakDict = new Dictionary<string, List<Break>>();
        Rosters = new Dictionary<(int, DateTime), Roster>();
        EmpShiftConnections = new List<EmployeeShift>();
        SingleRuleDict = new Dictionary<int, List<ShiftRuleSingle>>();
        RecurringRuleDict = new Dictionary<int, List<ShiftRuleRecurring>>();
        RosterRuleDict = new Dictionary<int, List<ShiftRuleRoster>>();
        EmployeeRosters = new List<EmployeeRoster>();
        DailyRosters = new List<DailyRoster>();
        DailyShiftCounters = new Dictionary<Guid, List<DailyShiftCounter>>();
        WeeklyShiftCounters = new Dictionary<Guid, List<WeeklyShiftCounter>>();
    }

    public RosterDataSet(Department department, DateTime startDate, DateTime endDate, IEnumerable<Employee> employees,
        IEnumerable<Roster> rosters, IEnumerable<DailyRoster> dailyRosters, IEnumerable<EmployeeRoster> employeeRosters,
        IEnumerable<Shift> shifts, IEnumerable<Break> breaks, IEnumerable<EmployeeShift> esCons,
        IEnumerable<ShiftRuleSingle> singleRules, IEnumerable<ShiftRuleRecurring> recurringRules, IEnumerable<ShiftRuleRoster> rosterRules,
        IEnumerable<DailyShiftCounter> dailyShiftCounters, IEnumerable<WeeklyShiftCounter> weeklyShiftCounters)
    {
        Department = department;
        StartDate = startDate;
        EndDate = endDate;
        Employees = employees.ToDictionary(e => e.ID, e => e);
        Shifts = shifts.ToDictionary(s => s.ID, s => s);
        BreakDict = breaks.GroupBy(b => b.ShiftID).ToDictionary(g => g.Key, g => g.ToList());
        Rosters = rosters.ToDictionary(r => (r.EmployeeID, r.Date), r => r);
        EmpShiftConnections = esCons;
        SingleRuleDict = singleRules.GroupBy(rule => rule.EmployeeID)
            .ToDictionary(group => group.Key, group => group.ToList());
        RecurringRuleDict = recurringRules.GroupBy(rule => rule.EmployeeID)
            .ToDictionary(group => group.Key, group => group.ToList());
        RosterRuleDict = rosterRules.GroupBy(rule => rule.EmployeeID)
            .ToDictionary(group => group.Key, group => group.ToList());

        EmployeeRosters = employeeRosters.ToList();
        DailyRosters = dailyRosters.ToList();

        DailyShiftCounters = dailyShiftCounters.GroupBy(counter => counter.RosterID)
            .ToDictionary(group => group.Key, group => group.ToList());

        WeeklyShiftCounters = weeklyShiftCounters.GroupBy(counter => counter.RosterID)
            .ToDictionary(group => group.Key, group => group.ToList());

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
    }

    private void SetFromShifts()
    {
        foreach (var (_, shift) in Shifts)
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
            Employees.TryGetValue(empShiftConnection.EmployeeID, out var employee);
            Shifts.TryGetValue(empShiftConnection.ShiftID, out var shift);

            empShiftConnection.Employee = employee;
            empShiftConnection.Shift = shift;

            if (employee is null || shift is null) continue;

            employee.Shifts.Add(shift);
            shift.Employees.Add(employee);
        }
    }

    private void SetFromEmployees()
    {
        foreach (var (_, employee) in Employees)
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
                if (!Shifts.TryGetValue(shiftRule.ShiftID ?? "", out var shift)) continue;
                
                shiftRule.Shift = shift;
                shift.RosterRules.Add(shiftRule);
            }
        }
    }

    private void SetFromRosters()
    {
        foreach (var (_, roster) in Rosters)
        {
            if (roster.DepartmentName == Department?.Name) roster.Department = Department;

            if (Employees.TryGetValue(roster.EmployeeID, out var employee))
            {
                employee.Rosters.Add(roster);
                roster.Employee = employee;
            }

            if (Shifts.TryGetValue(roster.ShiftID, out var shift))
            {
                roster.Shift = shift;
                shift.Rosters.Add(roster);
            }
        }
    }
}