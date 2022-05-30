using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace Uranus.Staff.Models;

public class Licence
{
    [PrimaryKey] public string Number { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool LF { get; set; }
    public bool LO { get; set; }
    public bool WP { get; set; }
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }


    [OneToOne(nameof(EmployeeID), nameof(Models.Employee.Licence), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }

    [OneToMany(nameof(LicenceImage.LicenceNumber), nameof(LicenceImage.Licence), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<LicenceImage> Images { get; set; }

    public Licence()
    {
        Number = string.Empty;
        IssueDate = DateTime.MinValue;
        ExpiryDate = DateTime.MinValue;
        Images = new List<LicenceImage>();
    }

    public Licence(string number, DateTime issueDate, DateTime expiryDate, bool lf, bool lo, bool wp, int employeeID, Employee? employee, List<LicenceImage> images)
    {
        Number = number;
        IssueDate = issueDate;
        ExpiryDate = expiryDate;
        LF = lf;
        LO = lo;
        WP = wp;
        EmployeeID = employeeID;
        Employee = employee;
        Images = images;
    }
}