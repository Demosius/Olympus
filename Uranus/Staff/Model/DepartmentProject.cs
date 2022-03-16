using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model;

public class DepartmentProject
{
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    [ForeignKey(typeof(Project))] public string ProjectName { get; set; }
}