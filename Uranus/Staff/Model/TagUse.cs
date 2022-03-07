using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Staff.Model;

public class TagUse
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    [ForeignKey(typeof(Employee))]
    public int EmployeeID { get; set; }
    [ForeignKey(typeof(TempTag))]
    public string TempTagRfID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public TempTag TempTag { get; set; }
    [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee Employee { get; set; }
}