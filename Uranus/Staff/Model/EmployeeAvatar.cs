using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;
using System.IO;

namespace Uranus.Staff.Model;

public class EmployeeAvatar : Image
{
    [OneToMany( nameof(Employee.AvatarName), nameof(Employee.Avatar),CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Employees { get; set; }

    public override string GetImageFilePath(StaffReader reader) => Path.Combine(reader.EmployeeAvatarDirectory, FileName);

}