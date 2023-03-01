﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Uranus.Staff.Models;

/// <summary>
/// Employee's weekly roster. Should reference a roster object for each day.
/// </summary>
public class EmployeeRoster : IEquatable<EmployeeRoster>, IComparable<EmployeeRoster>
{
    [PrimaryKey] public Guid ID { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    [ForeignKey(typeof(Shift))] public string ShiftID { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }
    [ForeignKey(typeof(DepartmentRoster))] public Guid DepartmentRosterID { get; set; }
    public DateTime StartDate { get; set; }
    public ERosterType RosterType { get; set; }

    [ForeignKey(typeof(Roster))] public Guid MondayRosterID { get; set; }
    [ForeignKey(typeof(Roster))] public Guid TuesdayRosterID { get; set; }
    [ForeignKey(typeof(Roster))] public Guid WednesdayRosterID { get; set; }
    [ForeignKey(typeof(Roster))] public Guid ThursdayRosterID { get; set; }
    [ForeignKey(typeof(Roster))] public Guid FridayRosterID { get; set; }
    [ForeignKey(typeof(Roster))] public Guid SaturdayRosterID { get; set; }
    [ForeignKey(typeof(Roster))] public Guid SundayRosterID { get; set; }

    [ManyToOne(nameof(DepartmentName), nameof(Models.Department.EmployeeRosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }
    [ManyToOne(nameof(ShiftID), nameof(Models.Shift.EmployeeRosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Shift? Shift { get; set; }
    [ManyToOne(nameof(EmployeeID), nameof(Models.Employee.EmployeeRosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }
    [ManyToOne(nameof(DepartmentRosterID), nameof(Models.DepartmentRoster.EmployeeRosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public DepartmentRoster? DepartmentRoster { get; set; }

    [OneToOne(nameof(MondayRosterID), nameof(Roster.EmployeeRoster), CascadeOperations = CascadeOperation.CascadeRead)]
    public Roster? MondayRoster { get; set; }
    [OneToOne(nameof(TuesdayRosterID), nameof(Roster.EmployeeRoster), CascadeOperations = CascadeOperation.CascadeRead)]
    public Roster? TuesdayRoster { get; set; }
    [OneToOne(nameof(WednesdayRosterID), nameof(Roster.EmployeeRoster), CascadeOperations = CascadeOperation.CascadeRead)]
    public Roster? WednesdayRoster { get; set; }
    [OneToOne(nameof(ThursdayRosterID), nameof(Roster.EmployeeRoster), CascadeOperations = CascadeOperation.CascadeRead)]
    public Roster? ThursdayRoster { get; set; }
    [OneToOne(nameof(FridayRosterID), nameof(Roster.EmployeeRoster), CascadeOperations = CascadeOperation.CascadeRead)]
    public Roster? FridayRoster { get; set; }
    [OneToOne(nameof(SaturdayRosterID), nameof(Roster.EmployeeRoster), CascadeOperations = CascadeOperation.CascadeRead)]
    public Roster? SaturdayRoster { get; set; }
    [OneToOne(nameof(SundayRosterID), nameof(Roster.EmployeeRoster), CascadeOperations = CascadeOperation.CascadeRead)]
    public Roster? SundayRoster { get; set; }

    [Ignore] public List<Shift> Shifts => Employee?.Shifts ?? new List<Shift>();

    public EmployeeRoster()
    {
        ID = Guid.NewGuid();
        DepartmentName = string.Empty;
        ShiftID = string.Empty;
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
        DepartmentRosterID = DepartmentRoster.ID;
    }
    public Roster? GetDaily(DayOfWeek weekDay)
    {
        return weekDay switch
        {
            DayOfWeek.Sunday => SundayRoster,
            DayOfWeek.Monday => MondayRoster,
            DayOfWeek.Tuesday => TuesdayRoster,
            DayOfWeek.Wednesday => WednesdayRoster,
            DayOfWeek.Thursday => ThursdayRoster,
            DayOfWeek.Friday => FridayRoster,
            DayOfWeek.Saturday => SaturdayRoster,
            _ => throw new ArgumentOutOfRangeException(nameof(weekDay), weekDay, null)
        };
    }

    public bool SetDaily(Roster dailyRoster)
    {
        var date = dailyRoster.Date;
        if (date < StartDate || date > StartDate.AddDays(6)) return false;

        switch (date.DayOfWeek)
        {
            case DayOfWeek.Sunday:
                SundayRoster = dailyRoster;
                break;
            case DayOfWeek.Monday:
                MondayRoster = dailyRoster;
                break;
            case DayOfWeek.Tuesday:
                TuesdayRoster = dailyRoster;
                break;
            case DayOfWeek.Wednesday:
                WednesdayRoster = dailyRoster;
                break;
            case DayOfWeek.Thursday:
                ThursdayRoster = dailyRoster;
                break;
            case DayOfWeek.Friday:
                FridayRoster = dailyRoster;
                break;
            case DayOfWeek.Saturday:
                SaturdayRoster = dailyRoster;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dailyRoster), dailyRoster, null);
        }
        return true;
    }

    private void FillMissingDailyRosters()
    {
        if (Department is null || Employee is null) throw new DataException("Employee Roster missing Department or Employee.");

        // Ensure all daily rosters exist.
        MondayRoster ??= new Roster(Department, Employee, StartDate);
        TuesdayRoster ??= new Roster(Department, Employee, StartDate.AddDays(1));
        WednesdayRoster ??= new Roster(Department, Employee, StartDate.AddDays(2));
        ThursdayRoster ??= new Roster(Department, Employee, StartDate.AddDays(3));
        FridayRoster ??= new Roster(Department, Employee, StartDate.AddDays(4));
        SaturdayRoster ??= new Roster(Department, Employee, StartDate.AddDays(5));
        SundayRoster ??= new Roster(Department, Employee, StartDate.AddDays(6));
    }

    public void ApplyShiftRules()
    {
        if (Employee is null) return;

        // Gather rules that apply to this weekly roster, from the employee.
        var rules = Employee.ShiftRules.Where(rule => rule.AppliesToWeek(StartDate)).ToList();

        FillMissingDailyRosters();

        MondayRoster?.ApplyShiftRules(rules);
        TuesdayRoster?.ApplyShiftRules(rules);
        WednesdayRoster?.ApplyShiftRules(rules);
        ThursdayRoster?.ApplyShiftRules(rules);
        FridayRoster?.ApplyShiftRules(rules);
        SaturdayRoster?.ApplyShiftRules(rules);
        SundayRoster?.ApplyShiftRules(rules);
    }

    /// <summary>
    /// Sets teh shift to the employee's default if they have one.
    /// </summary>
    public void SetDefault()
    {
        var shift = Employee?.DefaultShift;
        if (shift is null) return;

        Shift = shift;
    }

    public void SubCount(Shift shift) => DepartmentRoster?.SubCount(shift);

    public void AddCount(Shift shift) => DepartmentRoster?.AddCount(shift);

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