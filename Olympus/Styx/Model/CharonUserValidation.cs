using Olympus.Helios.Staff.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Styx.Model
{
    // For validating whether the current user can perform actions relating to a specified user/employee.
    // Employees and Users are linked, so the ability to CRUD users is based on the associated employee.
    public partial class Charon
    {
        // Can only change password when you are logged in. Otherwise see 'CanUpdateUser' to be able to Reset Paswword.
        public bool CanChangePassword(Employee employee)
        => employee.ID == UserEmployee.ID;
        
        public bool CanChangePassword(int employeeID)
        => employeeID == UserEmployee.ID;
        
        // Using Employee objects.
        public bool CanCreateUser(Employee employee)
        => CurrentUser.Role.CreateUser >= GetLevelDifference(employee) || UserEmployee == employee;
        

        public bool CanReadUser(Employee employee)
        => CurrentUser.Role.ReadUser >= GetLevelDifference(employee) || UserEmployee == employee;
        

        public bool CanUpdateUser(Employee employee)
        => CurrentUser.Role.UpdateUser >= GetLevelDifference(employee) || UserEmployee == employee;

        public bool CanDeleteUser(Employee employee)
        => CurrentUser.Role.DeleteUser >= GetLevelDifference(employee) || UserEmployee == employee;

        // Using Employee ID as int.
        public bool CanCreateUser(int employeeID)
        => CurrentUser.Role.CreateUser >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;

        public bool CanReadUser(int employeeID)
        => CurrentUser.Role.ReadUser >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;

        public bool CanUpdateUser(int employeeID)
        => CurrentUser.Role.UpdateUser >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;

        public bool CanDeleteUser(int employeeID)
        => CurrentUser.Role.DeleteUser >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;
    }
}
