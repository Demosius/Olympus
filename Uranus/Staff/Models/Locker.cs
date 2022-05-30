using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Models;

public class Locker
{
    [PrimaryKey] public string ID { get; set; }
    public string Location { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }

    [OneToOne(nameof(EmployeeID), nameof(Models.Employee.Locker), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }

    public Locker()
    {
        ID = string.Empty;
        Location = string.Empty;
    }

    public Locker(string id, string location, int employeeID, Employee? employee)
    {
        ID = id;
        Location = location;
        EmployeeID = employeeID;
        Employee = employee;
    }
}