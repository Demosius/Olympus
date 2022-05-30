using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Models;

public class EmployeeProject
{
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }
    [ForeignKey(typeof(Project))] public string ProjectName { get; set; }

    public EmployeeProject()
    {
        ProjectName = string.Empty;
    }

    public EmployeeProject(int employeeID, string projectName)
    {
        EmployeeID = employeeID;
        ProjectName = projectName;
    }
}