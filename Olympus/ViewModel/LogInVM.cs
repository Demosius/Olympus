using Olympus.ViewModel.Commands;
using System.ComponentModel;

namespace Olympus.ViewModel
{
    public class LogInVM : INotifyPropertyChanged
    {
        private string userID;
        public string UserID
        {
            get => userID;
            set
            {
                userID = value;
                OnPropertyChanged(nameof(UserID));
            }
        }
        private string password = "";
        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public LogInCommand LogInCommand { get; set; }

        public LogInVM()
        {
            LogInCommand = new(this);
        }

        public bool LogIn()
        {
            if (int.TryParse(UserID, out var id))
                return App.Charon.LogIn(id, Password);
            else 
                return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
