using System;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Staff.Model
{
    public class Project
    {
        [PrimaryKey]
        public string Name { get; set; }
        public EProject Reference { get; set; }
        public string ToolTip { get; set; }
        [ForeignKey(typeof(ProjectIcon))]
        public string IconName { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public ProjectIcon Icon { get; set; }

        [ManyToMany(typeof(DepartmentProject), "DepartmentName", "Projects", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Department> Departments { get; set; }
        [ManyToMany(typeof(EmployeeProject), "EmployeeID", "Projects", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Employee> Employees { get; set; }
        
        public Project() { }
             
        // Create project and projectIcon together.
        public Project(EProject eProject, string iconFileName, StaffReader reader, string toolTip = "")
        {
            Reference = eProject;
            Name = eProject.ToString();
            Icon = new(this, iconFileName);
            ToolTip = toolTip;
            Icon.FullPath = Icon.GetImageFilePath(reader);
            IconName = Icon.Name;
        }
    }
}
