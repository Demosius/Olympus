using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model;

public class Licence
{
    [PrimaryKey] public string Number { get; set; } 
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool LF { get; set; }
    public bool LO { get; set; }
    public bool WP { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }

    [OneToOne(nameof(EmployeeID), nameof(Model.Employee.Licence), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee Employee { get; set; }

    [OneToMany( nameof(LicenceImage.LicenceNumber),nameof(LicenceImage.Licence), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<LicenceImage> Images { get; set; }

}