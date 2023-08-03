using System.Threading.Tasks;
using Uranus.Staff.Models;

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

    public bool CanCreateTempTag() => User is not null;

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
    public async Task<bool> CanReadEmployeeAsync(int employeeID)
    {
        return User?.Role is not null &&
            User.Role.ReadEmployee >= await GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }

    public async Task<bool> CanReadEmployeeSensitiveAsync(int employeeID)
    {
        return User?.Role is not null &&
            User.Role.ReadEmployeeSensitive >= await GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }

    public async Task<bool> CanReadEmployeeVerySensitiveAsync(int employeeID)
    {
        return User?.Role is not null &&
            User.Role.ReadEmployeeVerySensitive >= await GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }

    public async Task<bool> CanUpdateEmployeeAsync(int employeeID)
    {
        return User?.Role is not null &&
            User.Role.UpdateEmployee >= await GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }

    public async Task<bool> CanDeleteEmployeeAsync(int employeeID)
    {
        return User?.Role is not null &&
            User.Role.DeleteEmployee >= await GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }
}