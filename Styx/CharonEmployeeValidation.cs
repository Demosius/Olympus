using Uranus.Staff.Model;

namespace Styx;

// For validating whether the current user can perform actions relating to a specified employee.
public partial class Charon
{
    // Using Employee objects.
    public bool CanCreateEmployee()
    {
        return CurrentUser?.Role is not null &&
               CurrentUser.Role.CreateEmployee;
    }

    public bool CanReadEmployee(Employee employee)
    {
        return CurrentUser?.Role is not null &&
            CurrentUser.Role.ReadEmployee >= GetLevelDifference(employee) || UserEmployee == employee;
    }

    public bool CanReadEmployeeSensitive(Employee employee)
    {
        return CurrentUser?.Role is not null &&
            CurrentUser.Role.ReadEmployeeSensitive >= GetLevelDifference(employee) || UserEmployee == employee;
    }

    public bool CanReadEmployeeVerySensitive(Employee employee)
    {
        return CurrentUser?.Role is not null &&
            CurrentUser.Role.ReadEmployeeVerySensitive >= GetLevelDifference(employee) || UserEmployee == employee;
    }

    public bool CanUpdateEmployee(Employee employee)
    {
        return CurrentUser?.Role is not null &&
            CurrentUser.Role.UpdateEmployee >= GetLevelDifference(employee) || UserEmployee == employee;
    }

    public bool CanDeleteEmployee(Employee employee)
    {
        return CurrentUser?.Role is not null &&
            CurrentUser.Role.DeleteEmployee >= GetLevelDifference(employee) || UserEmployee == employee;
    }

    // Using Employee ID as int.
    public bool CanReadEmployee(int employeeID)
    {
        return CurrentUser?.Role is not null &&
            CurrentUser.Role.ReadEmployee >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;
    }

    public bool CanReadEmployeeSensitive(int employeeID)
    {
        return CurrentUser?.Role is not null &&
            CurrentUser.Role.ReadEmployeeSensitive >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;
    }

    public bool CanReadEmployeeVerySensitive(int employeeID)
    {
        return CurrentUser?.Role is not null &&
            CurrentUser.Role.ReadEmployeeVerySensitive >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;
    }

    public bool CanUpdateEmployee(int employeeID)
    {
        return CurrentUser?.Role is not null &&
            CurrentUser.Role.UpdateEmployee >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;
    }

    public bool CanDeleteEmployee(int employeeID)
    {
        return CurrentUser?.Role is not null &&
            CurrentUser.Role.DeleteEmployee >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;
    }

}