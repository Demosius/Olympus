using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Staff.Models;

public class TagUse
{
    [PrimaryKey, AutoIncrement] public int ID { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }
    [ForeignKey(typeof(TempTag))] public string TempTagRF_ID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [ManyToOne(nameof(TempTagRF_ID), nameof(Models.TempTag.TagUse), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public TempTag? TempTag { get; set; }
    [ManyToOne(nameof(EmployeeID), nameof(Models.Employee.TagUse), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }

    public TagUse()
    {
        TempTagRF_ID = string.Empty;
    }

    public TagUse(int employeeID, string tempTagRFID, DateTime startDate, DateTime endDate, TempTag tempTag, Employee employee)
    {
        EmployeeID = employeeID;
        TempTagRF_ID = tempTagRFID;
        StartDate = startDate;
        EndDate = endDate;
        TempTag = tempTag;
        Employee = employee;
    }
}