using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Staff.Model
{
    public class Project
    {
        [PrimaryKey]
        public string Name { get; set; }
        public EProject EProject { get; set; }
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
        public Project(EProject eProject, string iconFileName, string toolTip = "")
        {
            EProject = eProject;
            Name = EnumConverter.ProjectToString(eProject);
            Icon = new ProjectIcon(this, iconFileName);
            ToolTip = toolTip;
        }
    }
}
