using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff.Model
{
    public class Clan
    {
        [PrimaryKey]
        public string Name { get; set; }
        [ForeignKey(typeof(Employee))]
        public int LeaderID { get; set; }
        [ForeignKey(typeof(Department))]
        public string DepartmentName { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Department Department { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Employee Leader { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Employee> Employees { get; set; } = new List<Employee> { };


    }
}
