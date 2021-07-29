using Olympus.Uranus.Staff.Model;
using Olympus.Uranus.Users.Model;
using Olympus.Styx.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using StaffRole = Olympus.Uranus.Staff.Model.Role;

namespace Olympus.Styx.ViewModel
{
    public class AlphaRegistrationVM : INotifyPropertyChanged
    {
        private const string GOOD_COLOUR = "Green";
        private const string WARN_COLOUR = "Orange";
        private const string BAD_COLOUR = "Red";

        public Employee Employee { get; set; } 
        public Department Department { get; set; }
        public StaffRole Role { get; set; }

        public string DepartmentName { get => Department.Name; set { Department.Name = value; OnPropertyChanged(nameof(DepartmentName)); } }

        private string password = "";
        public string Password { private get => password; set { password = value; CheckPasswords(); OnPropertyChanged(nameof(Password)); } }
        private string confirmPassword = "";
        public string ConfirmPassword { private get => confirmPassword; set { confirmPassword = value; CheckPasswords(); OnPropertyChanged(nameof(ConfirmPassword)); } }

        private string clrSixChar = "";
        public string Colour_SixChar { get => clrSixChar; set { clrSixChar = value; OnPropertyChanged(nameof(Colour_SixChar)); } }
        private string clrMatch = "";
        public string Colour_Match { get => clrMatch; set { clrMatch = value; OnPropertyChanged(nameof(Colour_Match)); } }
        private string clrNoSpace = "";
        public string Colour_NoSpace { get => clrNoSpace; set { clrNoSpace = value; OnPropertyChanged(nameof(Colour_NoSpace)); } }
        private string clrEightChar = "";
        public string Colour_EightChar { get => clrEightChar; set { clrEightChar = value; OnPropertyChanged(nameof(Colour_EightChar)); } }
        private string clrLower = "";
        public string Colour_Lower { get => clrLower; set { clrLower = value; OnPropertyChanged(nameof(Colour_Lower)); } }
        private string clrUpper = "";
        public string Colour_Upper { get => clrUpper; set { clrUpper = value; OnPropertyChanged(nameof(Colour_Upper)); } }
        private string clrNumber = "";
        public string Colour_Number { get => clrNumber; set { clrNumber = value; OnPropertyChanged(nameof(Colour_Number)); } }
        private string clrSpecial = "";
        public string Colour_Special { get => clrSpecial; set { clrSpecial = value; OnPropertyChanged(nameof(Colour_Special)); } }

        public bool PasswordGood { get; set; }

        public AlphaRegisterCommand AlphaRegisterCommand { get; set; }

        public AlphaRegistrationVM()
        {
            Employee = new Employee();
            Department = new Department();
            Role = new StaffRole();
            AlphaRegisterCommand = new AlphaRegisterCommand(this);
            CheckPasswords();
        }

        public void CheckPasswords()
        {
            bool isSix = Password.Length >= 6;
            bool isMatch = Password == ConfirmPassword;
            bool isNoSpace = !Password.Any(char.IsWhiteSpace);

            Colour_SixChar = isSix ? GOOD_COLOUR : BAD_COLOUR;
            Colour_Match = isMatch ? GOOD_COLOUR : BAD_COLOUR;
            Colour_NoSpace = isNoSpace ? GOOD_COLOUR : BAD_COLOUR;
            Colour_EightChar = (Password.Length >= 8) ? GOOD_COLOUR : WARN_COLOUR;
            Colour_Lower = (Password.Any(char.IsLower)) ? GOOD_COLOUR : WARN_COLOUR;
            Colour_Upper = (Password.Any(char.IsUpper)) ? GOOD_COLOUR : WARN_COLOUR;
            Colour_Number = (Password.Any(char.IsDigit)) ? GOOD_COLOUR : WARN_COLOUR;
            Colour_Special = (!Password.All(char.IsLetterOrDigit)) ? GOOD_COLOUR : WARN_COLOUR;

            PasswordGood = isSix && isMatch && isNoSpace;
        }

        public void Register()
        {
            Employee.DepartmentName = Department.Name;
            Role.DepartmentName = Department.Name;
            Department.HeadID = Employee.ID;
            App.Charon.RegisterAlphaUser(Employee, Department, Role, Password, ConfirmPassword);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
