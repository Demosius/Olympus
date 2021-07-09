using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff.Model
{
    public class Role
    {
        [PrimaryKey]
        public string Name { get; set; }
        [ForeignKey(typeof(Department))]
        public string DepartmentName { get; set; }
        public int Level { get; set; }
        [ForeignKey(typeof(Role))]
        public string ReportsToRoleName { get; set; }

        [ManyToOne]
        public Department Department { get; set; }
        [OneToOne]
        public Role ReprortsToRole { get; set; }
        [OneToMany]
        public List<Employee> Employees { get; set; }
        [OneToMany]
        public List<Role> Reports { get; set; } 
        
    }
}
