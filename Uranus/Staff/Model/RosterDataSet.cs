﻿using System;
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
    public Dictionary<int, List<ShiftRule>> ShiftRuleDict { get; set; }

    public List<EmployeeRoster> EmployeeRosters { get; set; }
    public List<DailyRoster> DailyRosters { get; set; }

    public RosterDataSet()
    {
        Employees = new Dictionary<int, Employee>();
        Shifts = new Dictionary<string, Shift>();
        BreakDict = new Dictionary<string, List<Break>>();
        Rosters = new Dictionary<(int, DateTime), Roster>();
        EmpShiftConnections = new List<EmployeeShift>();
        ShiftRuleDict = new Dictionary<int, List<ShiftRule>>();
        EmployeeRosters = new List<EmployeeRoster>();
        DailyRosters = new List<DailyRoster>();
    }

    public RosterDataSet(Department department, DateTime startDate, DateTime endDate, IEnumerable<Employee> employees,
        IEnumerable<Roster> rosters, IEnumerable<DailyRoster> dailyRosters, IEnumerable<EmployeeRoster> employeeRosters,
        IEnumerable<Shift> shifts, IEnumerable<Break> breaks, IEnumerable<EmployeeShift> esCons, IEnumerable<ShiftRule> shiftRules)
    {
        Department = department;
        StartDate = startDate;
        EndDate = endDate;
        Employees = employees.ToDictionary(e => e.ID, e => e);
        Shifts = shifts.ToDictionary(s => s.ID, s => s);
        BreakDict = breaks.GroupBy(b => b.ShiftID).ToDictionary(g => g.Key, g => g.ToList());
        Rosters = rosters.ToDictionary(r => (r.EmployeeID, r.Date), r => r);
        EmpShiftConnections = esCons;
        ShiftRuleDict = shiftRules.GroupBy(r => r.EmployeeID).ToDictionary(g => g.Key, g => g.ToList());

        EmployeeRosters = employeeRosters.ToList();
        DailyRosters = dailyRosters.ToList();

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

            if (!ShiftRuleDict.TryGetValue(employee.ID, out var shiftRules)) continue;

            employee.Rules = shiftRules;
            foreach (var shiftRule in shiftRules)
                shiftRule.Employee = employee;
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