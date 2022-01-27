using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model
{
    // Lists a rule/parameter for an employee. (e.g. Employee has every second thursday off, leaves early on mondays, etc.)
    // Each entry is another rule, and multiple rules can apply to a single employee.
    public class ShiftRule
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [ForeignKey(typeof(Employee))]
        public int EmployeeID { get; set; }
        public string Rule { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Employee Employee { get; set; }
    }
}
