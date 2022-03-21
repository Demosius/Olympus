using Uranus.Staff.Model;

namespace Styx;

// For validating whether the current user can perform actions relating to a specified user/employee.
// Employees and Users are linked, so the ability to CRUD users is based on the associated employee.
public partial class Charon
{
    // Can only change password when you are logged in. Otherwise see 'CanUpdateUser' to be able to Reset Password.
    public bool CanChangePassword(Employee employee) => CanChangePassword(employee.ID);

    public bool CanChangePassword(int employeeID)
    {
        if (CurrentUser is null) return false;
        return employeeID == UserEmployee?.ID;
    }

    // Using Employee objects.
    public bool CanCreateUser(Employee employee)
    {
        if (CurrentUser is null) return false;
        return CurrentUser.Role?.CreateUser >= GetLevelDifference(employee) || UserEmployee == employee;
    }


    public bool CanReadUser(Employee employee)
    {
        if (CurrentUser is null) return false;
        return CurrentUser.Role?.ReadUser >= GetLevelDifference(employee) || UserEmployee == employee;
    }


    public bool CanUpdateUser(Employee employee)
    {
        if (CurrentUser is null) return false;
        return CurrentUser.Role?.UpdateUser >= GetLevelDifference(employee) || UserEmployee == employee;
    }

    public bool CanDeleteUser(Employee employee)
    {
        if (CurrentUser is null) return false;
        return CurrentUser.Role?.DeleteUser >= GetLevelDifference(employee) || UserEmployee == employee;
    }

    // Using Employee ID as int.
    public bool CanCreateUser(int employeeID)
    {
        if (CurrentUser is null) return false;
        return CurrentUser.Role?.CreateUser >= GetLevelDifference(employeeID) || UserEmployee?.ID == employeeID;
    }

    public bool CanReadUser(int employeeID)
    {
        if (CurrentUser is null) return false;
        return CurrentUser.Role?.ReadUser >= GetLevelDifference(employeeID) || UserEmployee?.ID == employeeID;
    }

    public bool CanUpdateUser(int employeeID)
    {
        if (CurrentUser is null) return false;
        return CurrentUser.Role?.UpdateUser >= GetLevelDifference(employeeID) || UserEmployee?.ID == employeeID;
    }

    public bool CanDeleteUser(int employeeID)
    {
        if (CurrentUser is null) return false;
        return CurrentUser.Role?.DeleteUser >= GetLevelDifference(employeeID) || UserEmployee?.ID == employeeID;
    }



}