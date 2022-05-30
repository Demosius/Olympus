using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Models;

public class DepartmentProject
{
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    [ForeignKey(typeof(Project))] public string ProjectName { get; set; }

    public DepartmentProject()
    {
        DepartmentName = string.Empty;
        ProjectName = string.Empty;
    }

    public DepartmentProject(string departmentName, string projectName)
    {
        DepartmentName = departmentName;
        ProjectName = projectName;
    }
}