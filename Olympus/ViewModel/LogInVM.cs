using Olympus.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.ViewModel
{
    public class LogInVM : INotifyPropertyChanged
    {
        private int userID;
        public int UserID { get => userID; set { userID = value; OnPropertyChanged(nameof(UserID)); } }
        private string password = "";
        public string Password { get => password; set { password = value; OnPropertyChanged(nameof(Password)); } }

        public LogInCommand LogInCommand { get; set; }

        public LogInVM()
        {
            LogInCommand = new LogInCommand(this);
        }

        public bool LogIn()
        {
            return App.Charon.LogIn(UserID, Password);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
