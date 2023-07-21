using System;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Models;

public class TempTag
{
    // ReSharper disable once InconsistentNaming
    [PrimaryKey] public string RF_ID { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }

    [OneToMany(nameof(Models.TagUse.TempTagRF_ID), nameof(Models.TagUse.TempTag), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<TagUse> TagUse { get; set; }

    [OneToOne(nameof(EmployeeID), nameof(Models.Employee.TempTag), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
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

    public TagUse? SetEmployee(Employee employee, bool isNew = false)
    {
        Employee = employee;
        EmployeeID = employee.ID;

        Employee.TempTagRF_ID = RF_ID;
        Employee.TempTag = this;

        if (!isNew) return null;

        var usage = new TagUse
        {
            Employee = employee, 
            EmployeeID = EmployeeID,
            EndDate = null,
            StartDate = DateTime.Today,
            TempTag = this,
            TempTagRF_ID = RF_ID,
        };
        TagUse.Add(usage);
        return usage;

    }

    public void Unassign()
    {
        // Change applicable use.
        var empID = EmployeeID;
        var use = TagUse.FirstOrDefault(use => use.EmployeeID == empID && use.EndDate is null);
        if (use is not null) use.EndDate = DateTime.Today;
        // Change Employee object.
        if (Employee is not null)
        {
            Employee.TempTag = null;
            Employee.TempTagRF_ID = string.Empty;
        }
        // Change relevant tag data.
        Employee = null;
        EmployeeID = 0;
    }
}