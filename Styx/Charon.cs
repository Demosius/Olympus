﻿using StaffRole = Uranus.Staff.Model.Role;
using System;
using Uranus.Users.Model;
using Uranus.Staff.Model;
using Uranus.Users;
using Uranus.Staff;
using Uranus;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Styx
{
    public class PasswordException : Exception
    {
        public PasswordException(string message) : base(message) { }
    }

    public partial class Charon
    {
        private User currentUser;
        public User CurrentUser 
        { 
            get => currentUser;
            set
            {
                // Make sure user project icon paths are set.
                foreach (var p in value?.Employee?.Projects ?? new())
                    p.Icon.SetImageFilePath(staffReader);
                currentUser = value;
            }
        }
        public Employee UserEmployee
        {
            get
            {
                if (CurrentUser is null) return null;
                return CurrentUser.Employee;
            }
        }

        // User manipulations.
        private readonly UserChariot userChariot;
        private readonly UserCreator userCreator;
        private readonly UserReader userReader;
        private readonly UserUpdater userUpdater;
        // private readonly UserDeleter userDeleter;

        // Reading employees.
        private readonly StaffChariot staffChariot;
        private readonly StaffReader staffReader;
        private readonly StaffCreator staffCreator; // For alpha user registration.
        // private readonly StaffDeleter staffDeleter;

        public Charon(string solLocation)
        {
            userChariot = new(solLocation);
            userCreator = new(ref userChariot);
            userReader = new(ref userChariot);
            userUpdater = new(ref userChariot);
            // userDeleter = new UserDeleter(ref userChariot);

            staffChariot = new(solLocation);
            staffReader = new(ref staffChariot);
            staffCreator = new(ref staffChariot);
            // staffDeleter = new StaffDeleter(ref staffChariot);
        }

        public void DatabaseReset(string newSol)
        {
            LogOut();
            userChariot.ResetConnection(newSol);
            staffChariot.ResetConnection(newSol);
        }

        public int GetLevelDifference(Employee employee)
        {
            if (CurrentUser is null || UserEmployee is null) return 999;
            var targetRole = employee.Role;
            int up = 0, down = 0;
            if (UserEmployee.Role.LookDown(ref down, ref targetRole))
            {
                return -down;
            }
            return UserEmployee.Role.LookUp(ref up, ref down, UserEmployee.Role, ref targetRole) ? up : 999;
        }

        public int GetLevelDifference(int employeeID)
        {
            var employee = staffReader.Employee(employeeID, EPullType.FullRecursive);
            return GetLevelDifference(employee);
        }

        public bool ChangePassword(string newPassword, string confirmPassword, out string message)
        {
            if (!CanChangePassword(UserEmployee))
            {
                message = "Cannot change password of this employee.";
                return false;
            }
            if (!ValidatePassword(newPassword, confirmPassword, out message)) return false;

            // Set up new password for current user.
            var newLogin = GetNewLogin(CurrentUser.ID, newPassword);

            userUpdater.Login(newLogin);
            return true;
        }

        public bool ResetPassword(Employee employee)
        {
            // Make sure that the target employee has a User login, and that the current user
            // has permission to reset the password.
            if (CanUpdateUser(employee) && userReader.UserExists(employee.ID))
            {
                var newLogin = GetNewLogin(employee.ID, $"password{employee.ID}");

                userUpdater.Login(newLogin);
                return true;
            }
            return false;
        }

        public void LogOut()
        {
            CurrentUser = null;
        }

        public bool LogIn(int userID, string password)
        {
            var login = userReader.Login(userID);
            
            if (login is null) return false;
            if (!VerifyPassword(login, password)) return false;
            
            var user = userReader.User(userID);
            user.Employee = staffReader.Employee(userID, EPullType.IncludeChildren);
            CurrentUser = user;
            
            return true;

        }

        // Creating the original user. Only valid when there are no current employees/users/departments/etc.
        public bool RegisterAlphaUser(Employee employee, Department department, StaffRole staffRole, string password, string confirmPassword, out string message)
        {
            userCreator.AssureDBManagerRole();

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
                CurrentUser = newUser;
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
            CurrentUser = newUser;

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

            return CreateNewUser(employee.ID, "Default");
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
            return !userReader.UserExists(employee.ID) && CreateNewUser(employee.ID, roleName);
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
        /// <exception cref="PasswordException"></exception>
        private static bool ValidatePassword(string password, string confirmPassword, out string message)
        {
            // Check that password rules are adhered to.
            if (password.Length < 6) throw new("Password is too short.");

            // Check that the two different passwords are a match.
            if (password != confirmPassword) throw new("Passwords don't match.");

            // Check if the password contains spaces.
            if (password.Contains(' ')) throw new("Password cannot contain spaces.");

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
                message = null;

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

            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetNonZeroBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt, int nIterations, int nHash)
        {
            var saltBytes = Convert.FromBase64String(salt);

            using var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, nIterations);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(nHash));
        }
    }
}