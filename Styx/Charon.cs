using StaffRole = Uranus.Staff.Model.Role;
using Role = Uranus.Users.Model.Role;
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
        public User CurrentUser { get; set; }
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
            userChariot = new UserChariot(solLocation);
            userCreator = new UserCreator(ref userChariot);
            userReader = new UserReader(ref userChariot);
            userUpdater = new UserUpdater(ref userChariot);
            // userDeleter = new UserDeleter(ref userChariot);

            staffChariot = new StaffChariot(solLocation);
            staffReader = new StaffReader(ref staffChariot);
            staffCreator = new StaffCreator(ref staffChariot);
            // staffDeleter = new StaffDeleter(ref staffChariot);
        }

        public Charon(string solLocation, User user, Employee employee) : this(solLocation)
        {
            CurrentUser = user;
            if (user.Employee is null)
                user.Employee = employee;
            else if (user.ID != user.Employee.ID && user.ID == employee.ID)
                user.Employee = employee;
        }

        public void DatabaseReset()
        {
            LogOut();
            userChariot.ResetConnection();
            staffChariot.ResetConnection();
        }

        public int GetLevelDifference(Employee employee)
        {
            if (CurrentUser is null || UserEmployee is null) return 999;
            StaffRole targetRole = employee.Role;
            int up = 0, down = 0;
            if (UserEmployee.Role.LookDown(ref down, ref targetRole))
            {
                return -down;
            }
            if (UserEmployee.Role.LookUp(ref up, ref down, UserEmployee.Role, ref targetRole))
            {
                return up;
            }
            return 999;
        }

        public int GetLevelDifference(int employeeID)
        {
            Employee employee = staffReader.Employee(employeeID, PullType.FullRecursive);
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
            Login newLogin = GetNewLogin(CurrentUser.ID, newPassword);

            userUpdater.Login(newLogin);
            return true;
        }

        public bool ResetPassword(Employee employee)
        {
            // Make sure that the target employee has a User login, and that the current user
            // has permission to reset the password.
            if (CanUpdateUser(employee) && userReader.UserExists(employee.ID))
            {
                Login newLogin = GetNewLogin(employee.ID, $"password{employee.ID}");

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
            Login login = userReader.Login(userID);
            if (!(login is null))
            {
                if (VerifyPassword(login, password))
                {
                    CurrentUser = userReader.User(userID);
                    CurrentUser.Employee = staffReader.Employee(userID, PullType.FullRecursive);
                    return true;
                }
            }

            return false;
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

                _ = staffCreator.Employee(employee, PushType.FullRecursive);

                User newUser = new()
                {
                    ID = employee.ID
                };
                Login newLogin = GetNewLogin(employee.ID, password);

                Role userRole = userReader.Role("DatabaseManager");
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
                _ = userChariot.DeleteByKey<Employee>(employee.ID);
                _ = userChariot.DeleteByKey<Department>(department.Name);
                _ = userChariot.DeleteByKey<StaffRole>(staffRole.Name);
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
            Role role = userReader.Role("Default");
            User newUser = new()
            {
                ID = userID,
            };
            Login newLogin = GetNewLogin(userID, password);

            newUser.Role = role;
            newUser.Employee = staffReader.Employee(userID, PullType.FullRecursive);

            userCreator.User(newUser);
            userCreator.Login(newLogin);

            // Set user.
            CurrentUser = newUser;

            return true;
        }

        public bool CreateNewUser(Employee employee)
        {
            // Check that the employee doesn't already have an associated user.
            if (userReader.UserExists(employee.ID)) return false;

            // Create new user and login.
            userCreator.AssureDefaultRole();
            User newUser = new()
            {
                ID = employee.ID,
                RoleName = "Default"
            };
            Login newLogin = GetNewLogin(employee.ID, $"Password{employee.ID}");

            userCreator.User(newUser);
            userCreator.Login(newLogin);

            return true;
        }

        /// <summary>
        /// Check that password is correct for the user-login.
        /// </summary>
        private static bool VerifyPassword(Login login, string Password)
        {
            string checkHash = HashPassword(Password, login.Salt, 19291, 80);
            return checkHash == login.PasswordHash;
        }

        /// <summary>
        /// Validate password by comparing the two enterred passwords are equal, and checking other relevant rules.
        /// </summary>
        private static bool ValidatePassword(string password, string confirmPassword, out string message)
        {
            try
            {
                // Check that password rules are adhered to.
                if (password.Length < 6) throw new PasswordException("Password is too short.");

                // Check that the two different paswswords are a match.
                if (password != confirmPassword) throw new PasswordException("Passwords don't match.");

                // Check if the password contains spaces.
                if (password.Contains(' ')) throw new PasswordException("Password cannot contain spaces.");

                // Caution user against unsafe passwords.
                Regex regex = new(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\d])(?!.*\s).{8,255}$");
                if (!regex.IsMatch(password))
                {
                    message = $"Warning: Password is not very strong.\n" +
                                    $"\nRecommended:\n" +
                                    $"     •  Minimum 8 characters.\n" +
                                    $"     •  At least one lowercase letter.\n" +
                                    $"     •  At least one uppercase letter.\n" +
                                    $"     •  At least one digit.\n" +
                                    $"     •  At least one special character.\n" +
                                    $"\nRequired:\n" +
                                    $"     •  Minumum 6 characters.\n" +
                                    $"     •  No spaces.";
                }
                else
                    message = null;

                return true;
            }
            catch (PasswordException) { throw; }
            catch (Exception) { throw; }
        }

        private static Login GetNewLogin(int userID, string password)
        {
            string salt = GenerateSalt(16);
            string newPwdHash = HashPassword(password, salt, 19291, 80);
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
