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
    public bool CanCreateUser(int employeeID)
    {
        if (User is null) return false;
        return User.Role?.CreateUser >= GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }

    public bool CanReadUser(int employeeID)
    {
        if (User is null) return false;
        return User.Role?.ReadUser >= GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }

    public bool CanUpdateUser(int employeeID)
    {
        if (User is null) return false;
        return User.Role?.UpdateUser >= GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }

    public bool CanDeleteUser(int employeeID)
    {
        if (User is null) return false;
        return User.Role?.DeleteUser >= GetLevelDifference(employeeID) || Employee?.ID == employeeID;
    }
}