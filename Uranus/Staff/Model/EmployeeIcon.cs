using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;
using System.IO;

namespace Uranus.Staff.Model;

public class EmployeeIcon : Image
{
    [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Employees { get; set; }

    public override string GetImageFilePath(StaffReader reader) => Path.Combine(reader.EmployeeIconDirectory, FileName);

}