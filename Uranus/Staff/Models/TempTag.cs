using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Staff.Models;

public class TempTag
{
    // ReSharper disable once InconsistentNaming
    [PrimaryKey] public string RF_ID { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }

    [OneToMany(nameof(Models.TagUse.TempTagRFID), nameof(Models.TagUse.TempTag), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<TagUse> TagUse { get; set; }

    [OneToOne(nameof(EmployeeID), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }

    public TempTag()
    {
        RF_ID = string.Empty;
        TagUse = new List<TagUse>();
    }

    public TempTag(string rfID, int employeeID, List<TagUse> tagUse, Employee employee)
    {
        RF_ID = rfID;
        EmployeeID = employeeID;
        TagUse = tagUse;
        Employee = employee;
    }
}