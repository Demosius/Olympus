using Olympus.View;
using Olympus.ViewModel.Commands;
using System.ComponentModel;

namespace Olympus.ViewModel.Components
{
    public class UserHandlerVM : INotifyPropertyChanged
    {
        public OlympusVM ParentVM { get; set; }
        private string userGreeting;
        public string UserGreeting
        {
            get => userGreeting;
            set
            {
                userGreeting = value;
                OnPropertyChanged(nameof(UserGreeting));
            }
        }
        private string buttonString;
        public string ButtonString
        {
            get => buttonString;
            set
            {
                buttonString = value;
                OnPropertyChanged(nameof(ButtonString));
            }
        }

        public UserCommand UserCommand { get; set; }

        public UserHandlerVM()
        {
            CheckUser();
            UserCommand = new(this);
        }

        public UserHandlerVM(OlympusVM olympusVM) : this()
        {
            ParentVM = olympusVM;
        }

        public void CheckUser()
        {
            if (App.Charon.CurrentUser is null)
            {
                UserGreeting = "Who are you, and what are you doing here?";
                if (App.Helios.UserReader.UserCount() == 0)
                    ButtonString = "Register";
                else
                    ButtonString = "Log In";
            }
            else
            {
                UserGreeting = $"Hello, {App.Charon.UserEmployee.DisplayName}.";
                ButtonString = "Log Out";
            }
        }

        public void Register()
        {
            AlphaRegistrationWindow alpha = new();
            _ = alpha.ShowDialog();
            CheckUser();
        }

        public void LogIn()
        {
            LoginWindow login = new();
            _ = login.ShowDialog();
            CheckUser();
        }

        public void LogOut()
        {
            App.Charon.LogOut();
            CheckUser();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
