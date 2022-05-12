using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Staff.Model;

public enum ESingleRuleType
{
    Away,
    ArriveLate,
    LeaveEarly
}

public enum ELeaveType
{
    RDO = ERosterType.RDO,
    AnnualLeave = ERosterType.AL,
    PersonalCarersLeave = ERosterType.PCL
}

public class ShiftRuleSingle : ShiftRule
{

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan? Time { get; set; }
    public ESingleRuleType RuleType { get; set; }
    public ELeaveType LeaveType { get; set; }

    [ManyToOne(nameof(EmployeeID), nameof(Model.Employee.SingleRules), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }

    [Ignore] public string Summary => $"{RuleType} on {StartDate} to {EndDate}.";

    public ShiftRuleSingle()
    {
        StartDate = DateTime.Today.AddDays(1);
        EndDate = DateTime.Today.AddDays(1);
    }

    public ShiftRuleSingle(Employee employee) : this()
    {
        Employee = employee;
        EmployeeID = employee.ID;
    }
}