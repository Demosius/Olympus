using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Staff.Model;

public class Vehicle
{
    [PrimaryKey] public string Rego { get; set; }
    public string Colour { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public string Description { get; set; }

    [ManyToMany(typeof(EmployeeVehicle), nameof(EmployeeVehicle.VehicleRego), nameof(Employee.Vehicles), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Owners { get; set; }

    public Vehicle()
    {
        Rego = string.Empty;
        Colour = string.Empty;
        Make = string.Empty;
        Model = string.Empty;
        Description = string.Empty;
        Owners = new List<Employee>();
    }

    public Vehicle(string rego, string colour, string make, string model, string description, List<Employee> owners)
    {
        Rego = rego;
        Colour = colour;
        Make = make;
        Model = model;
        Description = description;
        Owners = owners;
    }
}