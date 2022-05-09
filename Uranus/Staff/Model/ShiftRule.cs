using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Staff.Model;

// Lists a rule/parameter for an employee. (e.g. Employee has every second thursday off, leaves early on mondays, etc.)
// Each entry is another rule, and multiple rules can apply to a single employee.
public class ShiftRule
{
    [PrimaryKey] public Guid ID { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }
    public string Description { get; set; }

    public ShiftRule()
    {
        ID = Guid.NewGuid();
        Description = string.Empty;
    }

    public ShiftRule(int employeeID, string description)
    {
        ID = Guid.NewGuid();
        EmployeeID = employeeID;
        Description = description;
    }
}