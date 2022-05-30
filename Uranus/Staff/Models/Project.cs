using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Staff.Models;

public class Project
{
    [PrimaryKey] public string Name { get; set; }
    public EProject Reference { get; set; }
    public string ToolTip { get; set; }
    [ForeignKey(typeof(ProjectIcon))] public string IconName { get; set; }

    [ManyToOne(nameof(IconName), nameof(ProjectIcon.Projects), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public ProjectIcon? Icon { get; set; }

    [ManyToMany(typeof(DepartmentProject), nameof(DepartmentProject.ProjectName), nameof(Department.Projects), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Department> Departments { get; set; }
    [ManyToMany(typeof(EmployeeProject), nameof(EmployeeProject.ProjectName), nameof(Employee.Projects), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Employees { get; set; }

    public Project()
    {
        Name = string.Empty;
        ToolTip = string.Empty;
        IconName = string.Empty;
        Departments = new List<Department>();
        Employees = new List<Employee>();
    }

    // Create project and projectIcon together.
    public Project(EProject eProject, string iconFileName, StaffReader reader, string toolTip = "")
    {
        Reference = eProject;
        Name = eProject.ToString();
        Icon = new ProjectIcon(this, iconFileName);
        ToolTip = toolTip;
        Icon.SetDirectory(reader.ProjectIconDirectory);
        IconName = Icon.Name;
        Departments = new List<Department>();
        Employees = new List<Employee>();
    }

    public Project(string name, EProject reference, string toolTip, string iconName, ProjectIcon icon, List<Department> departments, List<Employee> employees)
    {
        Name = name;
        Reference = reference;
        ToolTip = toolTip;
        IconName = iconName;
        Icon = icon;
        Departments = departments;
        Employees = employees;
    }
}