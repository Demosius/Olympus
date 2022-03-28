using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.IO;

namespace Uranus.Staff.Model;

public class EmployeeIcon : Image
{
    [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
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

    public override string GetImageFilePath(StaffReader reader) => Path.Combine(reader.EmployeeIconDirectory, FileName);

}