﻿using Uranus.Staff.Model;
using Olympus.ViewModel.Commands;
using System.ComponentModel;
using System.Linq;
using StaffRole = Uranus.Staff.Model.Role;

namespace Olympus.ViewModel
{
    public class AlphaRegistrationVM : INotifyPropertyChanged
    {
        private const string GoodColour = "Green";
        private const string WarnColour = "Orange";
        private const string BadColour = "Red";

        public Employee Employee { get; set; }
        public Department Department { get; set; }
        public StaffRole Role { get; set; }

        private string departmentName;
        public string DepartmentName
        {
            get => departmentName;
            set
            {
                departmentName = value;
                OnPropertyChanged(nameof(DepartmentName));
            }
        }

        private string roleName;
        public string RoleName
        {
            get => roleName;
            set
            {
                roleName = value;
                OnPropertyChanged(nameof(RoleName));
            }
        }

        private string employeeID;
        public string EmployeeID
        {
            get => employeeID;
            set
            {
                employeeID = value;
                OnPropertyChanged(nameof(EmployeeID));
            }
        }

        private string displayName;
        public string DisplayName
        {
            get => displayName;
            set
            {
                displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        private string firstName;
        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        private string lastName;
        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                OnPropertyChanged(nameof(lastName));
            }
        }

        private string rfID;
        public string RF_ID
        {
            get => rfID;
            set
            {
                rfID = value;
                OnPropertyChanged(nameof(RF_ID));
            }
        }

        private string pcID;
        public string PC_ID
        {
            get => pcID;
            set
            {
                pcID = value;
                OnPropertyChanged(nameof(PC_ID));
            }
        }

        private string phoneNumber;
        public string PhoneNumber
        {
            get => phoneNumber;
            set
            {
                phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }

        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string address;
        public string Address
        {
            get => address;
            set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        private decimal? payRate;
        public decimal? PayRate
        {
            get => payRate;
            set
            {
                payRate = value;
                OnPropertyChanged(nameof(PayRate));
            }
        }

        private int roleLevel;
        public int RoleLevel
        {
            get => roleLevel;
            set
            {
                roleLevel = value;
                OnPropertyChanged(nameof(RoleLevel));
            }
        }

        private string password = "";
        public string Password { private get => password; set { password = value; CheckPasswords(); OnPropertyChanged(nameof(Password)); } }
        private string confirmPassword = "";
        public string ConfirmPassword { private get => confirmPassword; set { confirmPassword = value; CheckPasswords(); OnPropertyChanged(nameof(ConfirmPassword)); } }

        private string clrSixChar = "";
        public string ColourSixChar { get => clrSixChar; set { clrSixChar = value; OnPropertyChanged(nameof(ColourSixChar)); } }
        private string clrMatch = "";
        public string ColourMatch { get => clrMatch; set { clrMatch = value; OnPropertyChanged(nameof(ColourMatch)); } }
        private string clrNoSpace = "";
        public string ColourNoSpace { get => clrNoSpace; set { clrNoSpace = value; OnPropertyChanged(nameof(ColourNoSpace)); } }
        private string clrEightChar = "";
        public string ColourEightChar { get => clrEightChar; set { clrEightChar = value; OnPropertyChanged(nameof(ColourEightChar)); } }
        private string clrLower = "";
        public string ColourLower { get => clrLower; set { clrLower = value; OnPropertyChanged(nameof(ColourLower)); } }
        private string clrUpper = "";
        public string ColourUpper { get => clrUpper; set { clrUpper = value; OnPropertyChanged(nameof(ColourUpper)); } }
        private string clrNumber = "";
        public string ColourNumber { get => clrNumber; set { clrNumber = value; OnPropertyChanged(nameof(ColourNumber)); } }
        private string clrSpecial = "";
        public string ColourSpecial { get => clrSpecial; set { clrSpecial = value; OnPropertyChanged(nameof(ColourSpecial)); } }

        public bool PasswordGood { get; set; }

        public AlphaRegisterCommand AlphaRegisterCommand { get; set; }
        public AlphaRegistrationVM()
        {
            AlphaRegisterCommand = new(this);
            CheckPasswords();
        }

        public void CheckPasswords()
        {
            var isSix = Password.Length >= 6;
            var isMatch = Password == ConfirmPassword;
            var isNoSpace = !Password.Any(char.IsWhiteSpace);

            ColourSixChar = isSix ? GoodColour : BadColour;
            ColourMatch = isMatch ? GoodColour : BadColour;
            ColourNoSpace = isNoSpace ? GoodColour : BadColour;
            ColourEightChar = Password.Length >= 8 ? GoodColour : WarnColour;
            ColourLower = Password.Any(char.IsLower) ? GoodColour : WarnColour;
            ColourUpper = Password.Any(char.IsUpper) ? GoodColour : WarnColour;
            ColourNumber = Password.Any(char.IsDigit) ? GoodColour : WarnColour;
            ColourSpecial = !Password.All(char.IsLetterOrDigit) ? GoodColour : WarnColour;

            PasswordGood = isSix && isMatch && isNoSpace;
        }

        public bool Register(out string message)
        {
            if (!int.TryParse(EmployeeID, out var id))
            {
                message = "Employee number must be a number.";
                return false;
            }
            Employee = new(id)
            {
                FirstName = FirstName,
                LastName = LastName,
                DisplayName = DisplayName,
                RF_ID = RF_ID,
                PC_ID = PC_ID,
                DepartmentName = DepartmentName,
                PhoneNumber = PhoneNumber,
                Email = Email,
                Address = Address,
                PayRate = PayRate
            };
            Department = new(DepartmentName)
            {
                HeadID = Employee.ID
            };
            Role = new(RoleName)
            {
                DepartmentName = Department.Name
            };
            return App.Charon.RegisterAlphaUser(Employee, Department, Role, Password, ConfirmPassword, out message);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
