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

}