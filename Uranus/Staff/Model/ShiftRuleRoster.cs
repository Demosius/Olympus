using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model;

public class ShiftRuleRoster : ShiftRule
{
    public string? ShiftID { get; set; }
    public bool Monday { get; set; }
    public bool Tuesday { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday { get; set; }
    public bool Friday { get; set; }
    public bool Saturday { get; set; }
    public bool Sunday { get; set; }

    [ManyToOne(nameof(ShiftID), nameof(Model.Shift.RosterRules), CascadeOperations = CascadeOperation.None)]
    public Shift? Shift { get; set; }
    [ManyToOne(nameof(EmployeeID), nameof(Model.Employee.RosterRules), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }
    
}