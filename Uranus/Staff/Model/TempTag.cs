using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model;

public class TempTag
{
    [PrimaryKey]
    // ReSharper disable once InconsistentNaming
    public string RF_ID { get; set; }
    [ForeignKey(typeof(Employee))]
    public int EmployeeID { get; set; }

    [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public TagUse TagUse { get; set; }
    [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee Employee { get; set; }
}