using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff
{
    public class Clan
    {
        public string Name { get; set; }
        public Department Department { get; set; }
        public Employee Leader { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee> { };

        public Clan() { }

        public Clan(string name, Department department, Employee leader)
        {
            Name = name;
            Department = department;
            Leader = leader;
        }
    }
}
