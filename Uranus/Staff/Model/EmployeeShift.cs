using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model;

// Denotes an employees eligibility for a particular shift.
public class EmployeeShift 
{
    [ForeignKey(typeof(Employee))]
    public int EmployeeID { get; set; }
    [ForeignKey(typeof(Shift))]
    public string ShiftName { get; set; }
}