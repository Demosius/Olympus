using Uranus.Staff.Model;

namespace Styx
{
    // For validating whether the current user can perform actions relating to a specified employee.
    public partial class Charon
    {
        // Using Employee objects.
        public bool CanCreateEmployee(Employee employee) 
        {
            if (CurrentUser is null) return false;
            return CurrentUser.Role.CreateEmployee >= GetLevelDifference(employee) || UserEmployee == employee;
        }
        
        public bool CanReadEmployee(Employee employee) 
        {
            if (CurrentUser is null) return false;
            return CurrentUser.Role.ReadEmployee >= GetLevelDifference(employee) || UserEmployee == employee;
        }
        
        public bool CanReadEmployeeSensitive(Employee employee) 
        {
            if (CurrentUser is null) return false;
            return CurrentUser.Role.ReadEmployeeSensitive >= GetLevelDifference(employee) || UserEmployee == employee;
        }
        
        public bool CanReadEmployeeVerySensitive(Employee employee)
        {
            if (CurrentUser is null) return false;
            return CurrentUser.Role.ReadEmployeeVerySensitive >= GetLevelDifference(employee) || UserEmployee == employee;
        }

        public bool CanUpdateEmployee(Employee employee)
        {
            if (CurrentUser is null) return false;
            return CurrentUser.Role.UpdateEmployee >= GetLevelDifference(employee) || UserEmployee == employee;
        }

        public bool CanDeleteEmployee(Employee employee)
        {
            if (CurrentUser is null) return false;
            return CurrentUser.Role.DeleteEmployee >= GetLevelDifference(employee) || UserEmployee == employee;
        }

        // Using Employee ID as int.
        public bool CanCreateEmployee(int employeeID) 
        {
            if (CurrentUser is null) return false;
            return CurrentUser.Role.CreateEmployee >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;
        }
        
        public bool CanReadEmployee(int employeeID)
        {
            if (CurrentUser is null) return false;
            return CurrentUser.Role.ReadEmployee >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;
        }

        public bool CanReadEmployeeSensitive(int employeeID)
        {
            if (CurrentUser is null) return false;
            return CurrentUser.Role.ReadEmployeeSensitive >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;
        }

        public bool CanReadEmployeeVerySensitive(int employeeID)
        {
            if (CurrentUser is null) return false;
            return CurrentUser.Role.ReadEmployeeVerySensitive >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;
        }

        public bool CanUpdateEmployee(int employeeID)
        {
            if (CurrentUser is null) return false;
            return CurrentUser.Role.UpdateEmployee >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;
        }

        public bool CanDeleteEmployee(int employeeID)
        {
            if (CurrentUser is null) return false;
            return CurrentUser.Role.DeleteEmployee >= GetLevelDifference(employeeID) || UserEmployee.ID == employeeID;
        }

    }
}
