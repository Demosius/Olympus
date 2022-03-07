using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model;

public class EmployeeVehicle
{
    [ForeignKey(typeof(Employee))]
    public int EmployeeID { get; set; }
    [ForeignKey(typeof(Vehicle))]
    public string VehicleRego { get; set; }
}