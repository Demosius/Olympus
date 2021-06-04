using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Staff
{
    public class Department
    {
        public string Name { get; set; }
        public Employee Head { get; set; }
        public List<Shift> Shifts { get; set; } = new List<Shift> { };
        public List<Employee> Employees { get; set; } = new List<Employee> { };
        public List<Clan> Clans { get; set; } = new List<Clan> { };

        public Department() { }

        public Department(string name, Employee head)
        {
            Name = name;
            Head = head;
        }
    }
}
