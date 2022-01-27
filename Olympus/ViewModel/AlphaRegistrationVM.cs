using Uranus.Staff.Model;
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

        public string DepartmentName { get => Department.Name; set { Department.Name = value; OnPropertyChanged(nameof(DepartmentName)); } }

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
            Employee = new();
            Department = new();
            Role = new();
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
            ColourEightChar = (Password.Length >= 8) ? GoodColour : WarnColour;
            ColourLower = (Password.Any(char.IsLower)) ? GoodColour : WarnColour;
            ColourUpper = (Password.Any(char.IsUpper)) ? GoodColour : WarnColour;
            ColourNumber = (Password.Any(char.IsDigit)) ? GoodColour : WarnColour;
            ColourSpecial = (!Password.All(char.IsLetterOrDigit)) ? GoodColour : WarnColour;

            PasswordGood = isSix && isMatch && isNoSpace;
        }

        public bool Register(out string message)
        {
            Employee.DepartmentName = Department.Name;
            Role.DepartmentName = Department.Name;
            Department.HeadID = Employee.ID;
            return App.Charon.RegisterAlphaUser(Employee, Department, Role, Password, ConfirmPassword, out message);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
