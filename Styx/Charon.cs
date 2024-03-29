﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Uranus;
using Uranus.Staff;
using Uranus.Staff.Models;
using Uranus.Users;
using Uranus.Users.Models;
using Role = Uranus.Users.Models.Role;
using StaffRole = Uranus.Staff.Models.Role;

namespace Styx;

public class PasswordException : Exception
{
    public PasswordException(string message) : base(message) { }
}

public partial class Charon
{
    private User? user;
    public User? User
    {
        get => user;
        set
        {
            // Make sure user project icon paths are set.
            foreach (var p in value?.Employee?.Projects ?? new List<Project>())
                p.Icon?.SetDirectory(staffReader.ProjectIconDirectory);
            user = value;
        }
    }
    public Employee? Employee => User?.Employee;

    // User manipulations.
    private readonly UserChariot userChariot;
    private readonly UserCreator userCreator;
    private readonly UserReader userReader;
    private readonly UserUpdater userUpdater;
    private readonly UserDeleter userDeleter;

    // Reading employees.
    private readonly StaffChariot staffChariot;
    private readonly StaffReader staffReader;
    private readonly StaffCreator staffCreator; // For alpha user registration.
    private readonly StaffUpdater staffUpdater;
    // private readonly StaffDeleter staffDeleter;

    public Charon(string solLocation)
    {
        userChariot = new UserChariot(solLocation);
        userCreator = new UserCreator(ref userChariot);
        userReader = new UserReader(ref userChariot);
        userUpdater = new UserUpdater(ref userChariot);
        userDeleter = new UserDeleter(ref userChariot);

        staffChariot = new StaffChariot(solLocation);
        staffReader = new StaffReader(ref staffChariot);
        staffCreator = new StaffCreator(ref staffChariot);
        staffUpdater = new StaffUpdater(ref staffChariot);
        // staffDeleter = new StaffDeleter(ref staffChariot);
    }

    public void DatabaseReset(string newSol)
    {
        LogOut();
        userChariot.ResetConnection(newSol);
        staffChariot.ResetConnection(newSol);
    }

    public int GetLevelDifference(StaffRole role)
    {
        if (User is null ||
            Employee?.Role is null) return 999;

        var level = 0;

        if (Employee.Role.LookDown(ref role))
            return role.Level - Employee.Role.Level;

        return Employee.Role.LookUp(ref level, Employee.Role, ref role) ? level - Employee.Role.Level : 999;
    }

    public int GetLevelDifference(Employee employee)
    {
        if (User is null ||
            Employee is null ||
            employee.Role is null ||
            Employee.Role is null) return 999;

        var targetRole = employee.Role;
        var level = 0;

        if (Employee.Role.LookDown(ref targetRole))
            return targetRole.Level - Employee.Role.Level;

        return Employee.Role.LookUp(ref level, Employee.Role, ref targetRole) ? level - Employee.Role.Level : 999;
    }

    public async Task<int> GetLevelDifference(int employeeID)
    {
        var employee = await staffReader.RoleStackEmployeeAsync(employeeID);
        return employee is null ? 999 : GetLevelDifference(employee);
    }

    public bool ChangePassword(string newPassword, string confirmPassword, out string message)
    {
        if (User is null || Employee is null)
        {
            message = "No current user.";
            return false;
        }
        if (!ValidatePassword(newPassword, confirmPassword, out message)) return false;

        // Set up new password for current user.
        var newLogin = GetNewLogin(User.ID, newPassword);

        userUpdater.Login(newLogin);
        return true;
    }

    public bool ResetPassword(Employee employee)
    {
        // Make sure that the target employee has a User login, and that the current user
        // has permission to reset the password.
        if (!CanUpdateUser(employee) || !userReader.UserExists(employee.ID)) return false;

        var newLogin = GetNewLogin(employee.ID, $"password{employee.ID}");

        userUpdater.Login(newLogin);
        return true;
    }

    public void LogOut()
    {
        User = null;
    }

    public async Task<bool> LogInAsync(int userID, string password)
    {
        var login = userReader.Login(userID);

        if (login is null) return false;
        if (!VerifyPassword(login, password)) return false;

        var newUser = userReader.User(userID);
        if (newUser is null) return false;

        newUser.Employee = await staffReader.EmployeeLogInAsync(userID);
        User = newUser;

        return true;

    }

    // Creating the original user. Only valid when there are no current employees/users/departments/etc.
    public bool RegisterAlphaUser(Employee employee, Department department, StaffRole staffRole, string password, string confirmPassword, out string message)
    {
        _ = userCreator.AssureDBManagerRoleAsync();

        // Make sure that the passwords are valid before continuing too far.
        if (!ValidatePassword(password, confirmPassword, out message)) return false;

        // Check that there are no users. (In theory there should also be no employees, but that should not prevent an alpha user creation.)
        if (userReader.UserCount() > 0) return false;

        try
        {
            department.Head = employee;
            staffRole.Department = department;
            employee.Role = staffRole;
            employee.Department = department;

            _ = staffCreator.Employee(employee, EPushType.FullRecursive);

            User newUser = new()
            {
                ID = employee.ID
            };
            var newLogin = GetNewLogin(employee.ID, password);

            var userRole = userReader.Role("DatabaseManager");
            newUser.Role = userRole;
            newUser.Employee = employee;

            userCreator.User(newUser);
            userCreator.Login(newLogin);

            // Set user.
            User = newUser;
        }
        catch (Exception)
        {
            // Skip validation checks (as there is no user to validate)
            // and delete what might have been created in previous process.
            _ = staffChariot.DeleteByKey<Employee>(employee.ID);
            _ = staffChariot.DeleteByKey<Department>(department.Name);
            _ = staffChariot.DeleteByKey<StaffRole>(staffRole.Name);
            _ = userChariot.DeleteByKey<User>(employee.ID);
            throw;
        }
        return true;
    }

    public bool RegisterNewUser(int userID, string password, string confirmPassword, out string message)
    {
        // Check that there is an employee with the associated ID, and not an existing user.
        if (userReader.UserExists(userID) || !staffReader.EmployeeExists(userID))
        {
            message = "There is already a user associated with that ID.";
            return false;
        }

        // Make sure password is valid and matches second password.
        if (!ValidatePassword(password, confirmPassword, out message)) return false;

        // Create new user and associated login.
        userCreator.AssureDefaultRole();
        var role = userReader.Role("Default");
        User newUser = new()
        {
            ID = userID,
        };
        var newLogin = GetNewLogin(userID, password);

        newUser.Role = role;
        newUser.Employee = staffReader.Employee(userID, EPullType.FullRecursive);

        userCreator.User(newUser);
        userCreator.Login(newLogin);

        // Set user.
        User = newUser;

        return true;
    }

    /// <summary>
    /// Assumes that the id and role name are valid and creates data entries for login and user for a new user.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public bool CreateNewUser(int id, string roleName)
    {
        User newUser = new()
        {
            ID = id,
            RoleName = roleName
        };
        var newLogin = GetNewLogin(id, $"{id}");

        userCreator.User(newUser);
        userCreator.Login(newLogin);

        return true;
    }

    /// <summary>
    /// Creates a new user based on the given employee with the default user role.
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    public bool CreateNewUser(Employee employee)
    {
        // Check that the employee doesn't already have an associated user.
        if (userReader.UserExists(employee.ID)) return false;

        // Create new user and login.
        userCreator.AssureDefaultRole();

        if (!CreateNewUser(employee.ID, "Default")) return false;

        employee.IsUser = true;
        staffChariot.Update(employee);

        return true;

    }

    /// <summary>
    /// Creates a new user based on the given employee, and assigns it the given user role, assuming that it exists.
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public bool CreateNewUser(Employee employee, string roleName)
    {
        // Check if role exists.
        var role = userReader.Role(roleName);
        if (role is null) return CreateNewUser(employee);

        // Check that the employee doesn't already have an associated user. Create user if it doesn't.
        if (userReader.UserExists(employee.ID))
        {
            employee.IsUser = true;
            return false;
        }

        if (!CreateNewUser(employee.ID, roleName)) return false;

        employee.IsUser = true;
        staffUpdater.Employee(employee);
        return true;
    }

    /// <summary>
    /// Deactivates the given user. Deletes the user and login from the appropriate database, and changes the associated data to reflect that they are no longer a user.
    /// </summary>
    /// <param name="targetUser"></param>
    public async Task<bool> DeactivateUserAsync(User targetUser)
    {
        var isSuccess = await DeactivateUserAsync(targetUser.ID);
        if (isSuccess && targetUser.Employee is not null) targetUser.Employee.IsUser = false;
        return isSuccess;
    }

    /// <summary>
    /// Deactivates the given user. Deletes the user and login from the appropriate database, and changes the associated data to reflect that they are no longer a user.
    /// </summary>
    /// <param name="userID"></param>
    public async Task<bool> DeactivateUserAsync(int userID)
    {
        // Make sure the current user can (has permission to) deactivate the given user.
        if (!await CanDeleteUserAsync(userID)) return false;

        // Remove from user database.
        // Edit employee in staff database only if user is deleted.
        return userDeleter.User(userID) && staffUpdater.DeactivateUser(userID);
    }

    /// <summary>
    /// Check that password is correct for the user-login.
    /// </summary>
    private static bool VerifyPassword(Login login, string password)
    {
        var checkHash = HashPassword(password, login.Salt, 19291, 80);
        return checkHash == login.PasswordHash;
    }

    /// <summary>
    /// Validate password by comparing the two entered passwords are equal, and checking other relevant rules.
    /// </summary>
    private static bool ValidatePassword(string password, string confirmPassword, out string message)
    {
        // Check that password rules are adhered to.
        if (password.Length < 6)
        {
            message = "Password is too short.";
            return false;
        }

        // Check that the two different passwords are a match.
        if (password != confirmPassword)
        {
            message = "Passwords don't match.";
            return false;
        }

        // Check if the password contains spaces.
        if (password.Contains(' '))
        {
            message = "Password cannot contain spaces.";
            return false;
        }

        // Caution user against unsafe passwords.
        Regex regex = new(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\d])(?!.*\s).{8,255}$");
        if (!regex.IsMatch(password))
        {
            message = "Warning: Password is not very strong.\n" +
                      "\nRecommended:\n" +
                      "     •  Minimum 8 characters.\n" +
                      "     •  At least one lowercase letter.\n" +
                      "     •  At least one uppercase letter.\n" +
                      "     •  At least one digit.\n" +
                      "     •  At least one special character.\n" +
                      "\nRequired:\n" +
                      "     •  Minimum 6 characters.\n" +
                      "     •  No spaces.";
        }
        else
            message = string.Empty;

        return true;
    }

    private static Login GetNewLogin(int userID, string password)
    {
        var salt = GenerateSalt(16);
        var newPwdHash = HashPassword(password, salt, 19291, 80);
        Login newLogin = new()
        {
            UserID = userID,
            Salt = salt,
            PasswordHash = newPwdHash
        };
        return newLogin;
    }

    public static string GenerateSalt(int nSalt)
    {
        var saltBytes = new byte[nSalt];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetNonZeroBytes(saltBytes);
        }

        return Convert.ToBase64String(saltBytes);
    }

    public static string HashPassword(string password, string salt, int nIterations, int nHash)
    {
        var saltBytes = Convert.FromBase64String(salt);

        using var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, nIterations);
        return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(nHash));
    }

    /// <summary>
    /// Assigns the given role to the given user, changing it in the database, and on the given object.
    /// </summary>
    /// <param name="targetUser"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public bool SetRole(User targetUser, Role role)
    {
        if (targetUser.Role is null || !CanAssignUserRole(targetUser.Role, role)) return false;

        var oldRole = targetUser.Role;

        targetUser.Role = role;
        targetUser.RoleName = role.Name;

        if (userUpdater.User(targetUser) > 0)
            return true;

        targetUser.Role = oldRole;
        targetUser.RoleName = oldRole.Name;
        return false;
    }
}