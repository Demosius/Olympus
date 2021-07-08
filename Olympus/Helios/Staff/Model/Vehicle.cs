using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff.Model
{
    public class Vehicle
    {
        public string Rego { get; set; }
        public string Colour { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }

        public List<Employee> Owners { get; set; } = new List<Employee> { };

        public Vehicle() { }

        public Vehicle(string rego, string colour, string make, string model, string description)
        {
            Rego = rego;
            Colour = colour;
            Make = make;
            Model = model;
            Description = description;
        }
    }
}
