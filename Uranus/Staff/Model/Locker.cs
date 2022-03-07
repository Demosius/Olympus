using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model;

public class Locker
{
    [PrimaryKey]
    public string ID { get; set; }
    public string Location { get; set; }
    [ForeignKey(typeof(Employee))]
    public int EmployeeID { get; set; }

    [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee Employee { get; set; }

}