using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Staff.Model;

public class EmployeeInductionReference
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }
    [ForeignKey(typeof(Induction))] public string InductionType { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }

    [ManyToOne(nameof(EmployeeID), nameof(Model.Employee.InductionReferences), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee Employee { get; set; }
    [ManyToOne(nameof(InductionType), nameof(Model.Induction.EmployeeReferences), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Induction Induction { get; set; }
}