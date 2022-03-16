using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Staff.Model;

public class TempTag
{
    // ReSharper disable once InconsistentNaming
    [PrimaryKey] public string RF_ID { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }

    [OneToMany(nameof(Model.TagUse.TempTagRFID), nameof(Model.TagUse.TempTag), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<TagUse> TagUse { get; set; }

    [OneToOne(nameof(EmployeeID), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee Employee { get; set; }
}