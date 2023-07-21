using System.Threading.Tasks;
using Uranus.Staff.Models;

namespace Styx;

// For validating whether the current user can perform actions relating to a specified user/employee.
// Employees and Users are linked, so the ability to CRUD users is based on the associated employee.
public partial class Charon
{
    // Can only change password when you are logged in. Otherwise see 'CanUpdateUser' to be able to Reset Password.
    public bool CanChangePassword(Employee employee) => CanChangePassword(employee.ID);

    public bool CanChangePassword(int employeeID)
    {
        if (User is null) return false;
        return employeeID == Employee?.ID;
    }

    // Using Employee objects.
    public bool CanCreateUser(Employee employee)
    {
        if (User is null) return false;
        return User.Role?.CreateUser >= GetLevelDifference(employee) || Employee == employee;
    }


    public bool CanReadUser(Employee employee)
    {
        if (User is null) return false;
        return User.Role?.ReadUser >= GetLevelDifference(employee) || Employee == employee;
    }


    public bool CanUpdateUser(Employee employee)
    {
        if (User is null) return false;
        return User.Role?.UpdateUser >= GetLevelDifference(employee) || Employee == employee;
    }

    public bool CanDeleteUser(Employee employee)
    {
        if (User is null) return false;
        return User.Role?.DeleteUser >= GetLevelDifference(employee) || Employee == employee;
    }

    // Using Employee ID as int.
    public async Task<bool> CanCreateUserAsync(int employeeID)
    {
        if (User is null) return false;
        return User.Role?.CreateUser >= await GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }

    public async Task<bool> CanReadUserAsync(int employeeID)
    {
        if (User is null) return false;
        return User.Role?.ReadUser >= await GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }

    public async Task<bool> CanUpdateUserAsync(int employeeID)
    {
        if (User is null) return false;
        return User.Role?.UpdateUser >= await GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }

    public async Task<bool> CanDeleteUserAsync(int employeeID)
    {
        if (User is null) return false;
        return User.Role?.DeleteUser >= await GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }
}