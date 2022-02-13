using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model
{
    public enum ERosterType
    {
        Standard,
        AL,
        PCL,
        RDO
    }

    public class Roster
    {
        [PrimaryKey] public Guid ID { get; set; }
        [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }
        [ForeignKey(typeof(Shift))] public string ShiftName { get; set; }
        [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
        public DayOfWeek Day { get; set; }
        public string Date { get; set; }
        public ERosterType RosterType { get; set; }

        [ManyToOne(nameof(EmployeeID), nameof(Model.Employee.Rosters),CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Employee Employee { get; set; }
        [ManyToOne(nameof(ShiftName), nameof(Model.Shift.Rosters), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
        public Shift Shift { get; set; }
        [ManyToOne(nameof(DepartmentName), nameof(Model.Department.Rosters), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
        public Department Department { get; set; }

        public Roster()
        {
            ID = Guid.NewGuid();
        }
    }
}
