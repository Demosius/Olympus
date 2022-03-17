using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model;

public class EmployeeVehicle
{
    [ForeignKey(typeof(Employee))] public int EmployeeID { get; set; }
    [ForeignKey(typeof(Vehicle))] public string VehicleRego { get; set; }

    public EmployeeVehicle()
    {
        VehicleRego = string.Empty;
    }

    public EmployeeVehicle(int employeeID, string vehicleRego)
    {
        EmployeeID = employeeID;
        VehicleRego = vehicleRego;
    }
}