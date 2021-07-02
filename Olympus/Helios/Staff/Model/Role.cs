using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff
{
    public class Role
    {
        public string Name { get; set; }
        public Department Department { get; set; }
        public int Level { get; set; }
        private Role _reportsTo;
        public Role ReportsTo 
        {
            get { return _reportsTo; }
            set
            {
                if (value.Level > Level) _reportsTo = value;
            }
        }
        public List<Employee> Employees { get; set; } = new List<Employee> { }; // List of employees belonging to this role.
        public List<Role> Reports { get; set; } = new List<Role> { };           // List of roles that report to this one.

        public Role() { }

        public Role(string name, Department department, int level, Role reportsTo)
        {
            Name = name;
            Department = department;
            Level = level;
            ReportsTo = reportsTo;
        }
    }
}
