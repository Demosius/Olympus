using Uranus.Staff.Model;

namespace Styx;

// For validating whether the current user can perform actions relating to a specified employee.
public partial class Charon
{
    // Using Employee objects.
    public bool CanCreateEmployee() => User?.Role?.CreateEmployee ?? false;

    public bool CanReadEmployee(Employee employee)
    {
        return User?.Role is not null &&
            User.Role.ReadEmployee >= GetLevelDifference(employee) || Employee == employee;
    }

    public bool CanReadEmployeeSensitive(Employee employee)
    {
        return User?.Role is not null &&
            User.Role.ReadEmployeeSensitive >= GetLevelDifference(employee) || Employee == employee;
    }

    public bool CanReadEmployeeVerySensitive(Employee employee)
    {
        return User?.Role is not null &&
            User.Role.ReadEmployeeVerySensitive >= GetLevelDifference(employee) || Employee == employee;
    }
    
    public bool CanUpdateEmployee(Role role)
    {
        return User?.Role is not null &&
               User.Role.UpdateEmployee >= GetLevelDifference(role);
    }

    public bool CanUpdateEmployee(Employee employee)
    {
        return User?.Role is not null &&
            User.Role.UpdateEmployee >= GetLevelDifference(employee) || Employee == employee;
    }

    public bool CanDeleteEmployee(Employee employee)
    {
        return User?.Role is not null &&
            User.Role.DeleteEmployee >= GetLevelDifference(employee) || Employee == employee;
    }

    // Using Employee ID as int.
    public bool CanReadEmployee(int employeeID)
    {
        return User?.Role is not null &&
            User.Role.ReadEmployee >= GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }

    public bool CanReadEmployeeSensitive(int employeeID)
    {
        return User?.Role is not null &&
            User.Role.ReadEmployeeSensitive >= GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }

    public bool CanReadEmployeeVerySensitive(int employeeID)
    {
        return User?.Role is not null &&
            User.Role.ReadEmployeeVerySensitive >= GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }

    public bool CanUpdateEmployee(int employeeID)
    {
        return User?.Role is not null &&
            User.Role.UpdateEmployee >= GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }

    public bool CanDeleteEmployee(int employeeID)
    {
        return User?.Role is not null &&
            User.Role.DeleteEmployee >= GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }
}