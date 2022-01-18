using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Staff.Model
{
    public class Project
    {
        [PrimaryKey]
        public EProject EProject { get; set; }
        public string Name { get; set; }
        public string ToolTip { get; set; }
        [ForeignKey(typeof(ProjectIcon))]
        public string IconName { get; set; }

        [OneToOne(inverseProperty: "Project", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public ProjectIcon Icon { get; set; }

        [ManyToMany(typeof(DepartmentProject), "DepartmentName", "Projects", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Department> Departments { get; set; }
        [ManyToMany(typeof(EmployeeProject), "EmployeeID", "Projects", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Employee> Employees { get; set; }

        public Project() { }
             
        // Create project and projectIcon together.
        public Project(EProject eProject, string iconFileName, StaffReader reader, string toolTip = "")
        {
            EProject = eProject;
            Name = eProject.ToString();
            Icon = new ProjectIcon(this, iconFileName);
            ToolTip = toolTip;
            Icon.FullPath = Icon.GetImageFilePath(reader);
            IconName = Icon.Name;
        }
    }
}
