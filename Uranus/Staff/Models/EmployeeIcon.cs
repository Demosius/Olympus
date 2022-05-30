using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Staff.Models;

public class EmployeeIcon : Image
{
    [OneToMany(nameof(Employee.IconName), nameof(Employee.Icon), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Employees { get; set; }

    public EmployeeIcon()
    {
        Employees = new List<Employee>();
    }

    public EmployeeIcon(List<Employee> employees)
    {
        Employees = employees;
    }

    public EmployeeIcon(string fullPath, string name, string fileName, List<Employee> employees) : base(fullPath, name, fileName)
    {
        Employees = employees;
    }

    public EmployeeIcon(Image image)
    {
        Name = image.Name;
        FileName = image.FileName;
        Employees = new List<Employee>();
    }
}