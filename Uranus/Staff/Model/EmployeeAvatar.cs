using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Staff.Model;

public class EmployeeAvatar : Image
{
    [OneToMany(nameof(Employee.AvatarName), nameof(Employee.Avatar), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Employees { get; set; }

    public EmployeeAvatar()
    {
        Employees = new List<Employee>();
    }

    public EmployeeAvatar(Image image) : this()
    {
        Name = image.Name;
        FileName = image.FileName;
    }
}