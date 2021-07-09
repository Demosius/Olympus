using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Staff.Model
{
    public class Department
    {
        [PrimaryKey]
        public string Name { get; set; }
        [ForeignKey(typeof(Employee))]
        public int HeadID { get; set; }

        [OneToOne]
        public Employee Head { get; set; }
        [OneToMany]
        public List<Shift> Shifts { get; set; } = new List<Shift> { };
        [OneToMany]
        public List<Employee> Employees { get; set; } = new List<Employee> { };
        [OneToMany]
        public List<Clan> Clans { get; set; } = new List<Clan> { };
        [ManyToMany(typeof(Employee))]
        public List<Employee> EmployeesCanBorrow { get; set; }
    }
}
