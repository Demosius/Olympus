using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model;

public class ShiftRuleSingle : ShiftRule
{
    public enum ShiftRuleType
    {
        Away,
        ArriveLate,
        LeaveEarly
    }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? Time { get; set; }
    public ShiftRuleType RuleType { get; set; }

    [ManyToOne(nameof(EmployeeID), nameof(Model.Employee.SingleRules), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }


}