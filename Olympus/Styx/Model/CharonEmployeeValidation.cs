using Olympus.Uranus.Staff.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Styx.Model
{
    // For validating whether the current user can perform actions relating to a specified employee.
    public partial class Charon
    {
        // Using Employee objects.
        public bool CanCreateEmployee(Employee employee) 
        => CurrentUser.Role.CreateEmployee >= GetLevelDifference(employee) || UserEmployee == employee;
        
        public bool CanReadEmployee(Employee employee) 
        => CurrentUser.Role.ReadEmployee >= GetLevelDifference(employee) || UserEmployee == employee;
        
        public bool CanReadEmployeeSensitive(Employee employee) 
        =>  CurrentUser.Role.ReadEmployeeSensitive >= GetLevelDifference(employee) || UserEmployee == employee;
        
        public bool CanReadEmployeeVerySensitive(Employee employee)
        => CurrentUser.Role.ReadEmployeeVerySensitive >= GetLevelDifference(employee) || UserEmployee == employee;

        public bool CanUpdateEmployee(Employee employee)
        => CurrentUser.Role.UpdateEmployee >= GetLevelDifference(employee) || UserEmployee == employee;

        public bool CanDeleteEmployee(Employee employee)
        => CurrentUser.Role.DeleteEmployee >= GetLevelDifference(employee) || UserEmployee == employee;

        // Using Employee ID as int.
        public bool CanCreateEmployee(int employeeID) 
        => CurrentUser.Role.CreateEmployee >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;
        
        public bool CanReadEmployee(int employeeID)
        => CurrentUser.Role.ReadEmployee >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;

        public bool CanReadEmployeeSensitive(int employeeID)
        => CurrentUser.Role.ReadEmployeeSensitive >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;

        public bool CanReadEmployeeVerySensitive(int employeeID)
        => CurrentUser.Role.ReadEmployeeVerySensitive >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;

        public bool CanUpdateEmployee(int employeeID)
        => CurrentUser.Role.UpdateEmployee >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;

        public bool CanDeleteEmployee(int employeeID)
        => CurrentUser.Role.DeleteEmployee >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;
    }
}
