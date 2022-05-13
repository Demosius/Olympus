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

    [ManyToOne(nameof(EmployeeID), nameof(Model.Employee.SingleRules),
        CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }

    [Ignore] public string Summary {
        get
        {
            return RuleType switch
            {
                ESingleRuleType.Away => StartDate == EndDate
                    ? $"Away - {LeaveType}, on {StartDate:ddd, d MMM yy}."
                    : $"Away - {LeaveType}, from {StartDate:ddd, d MMM yy} to {EndDate:ddd, d MMM yy}.",
                ESingleRuleType.ArriveLate => StartDate == EndDate
                    ? $"Arrive late ({Time}) - on {StartDate:ddd, d MMM yy}."
                    : $"Arrive late ({Time}) - from {StartDate:ddd, d MMM yy} to {EndDate:ddd, d MMM yy}.",
                ESingleRuleType.LeaveEarly => StartDate == EndDate
                    ? $"Leave early ({Time}) - on {StartDate:ddd, d MMM yy}."
                    : $"Leave early ({Time}) - from {StartDate:ddd, d MMM yy} to {EndDate:ddd, d MMM yy}.",
                _ => throw new ArgumentOutOfRangeException($"{RuleType} rule type not acounted for.")
            };
        }
    }

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

    public ShiftRuleSingle Copy()
    {
        return new ShiftRuleSingle
        {
            ID = ID,
            Employee = Employee,
            StartDate = StartDate,
            EndDate = EndDate,
            Time = Time,
            RuleType = RuleType,
            LeaveType = LeaveType,
            EmployeeID = EmployeeID,
            Description = Description
        };
    }
}