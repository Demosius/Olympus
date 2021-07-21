using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff.Model
{
    public class Vehicle
    {
        [PrimaryKey]
        public string Rego { get; set; }
        public string Colour { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }

        [ManyToMany(typeof(EmployeeVehicle), "EmployeeID", "Vehicles", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Employee> Owners { get; set; } 

    }
}
