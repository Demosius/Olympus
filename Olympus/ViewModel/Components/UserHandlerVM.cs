using Olympus.Styx.View;
using Olympus.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            UserCommand = new UserCommand(this);
        }

        public UserHandlerVM(OlympusVM olympusVM) : this()
        {
            ParentVM = olympusVM;
        }

        private void CheckUser()
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
            AlphaRegistrationWindow alpha = new AlphaRegistrationWindow();
            alpha.ShowDialog();
        }

        public void LogIn()
        {

        }

        public void LogOut()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
