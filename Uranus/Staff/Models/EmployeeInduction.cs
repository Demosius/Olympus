using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Staff.Models;

public class EmployeeInductionReference
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }
    [ForeignKey(typeof(Induction))] public string InductionType { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }

    [ManyToOne(nameof(EmployeeID), nameof(Models.Employee.InductionReferences), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }
    [ManyToOne(nameof(InductionType), nameof(Models.Induction.EmployeeReferences), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Induction? Induction { get; set; }

    public EmployeeInductionReference()
    {
        InductionType = string.Empty;
        IssueDate = DateTime.MinValue;
        ExpiryDate = DateTime.MinValue;
    }

    public EmployeeInductionReference(int id, int employeeID, string inductionType, DateTime issueDate, DateTime expiryDate, Employee employee, Induction induction)
    {
        ID = id;
        EmployeeID = employeeID;
        InductionType = inductionType;
        IssueDate = issueDate;
        ExpiryDate = expiryDate;
        Employee = employee;
        Induction = induction;
    }
}