using Pantheon.ViewModel.Commands;
using System.ComponentModel;
using System.Windows;

namespace Pantheon.ViewModel
{
    public class LoginVM : INotifyPropertyChanged
    {
        private string userCode;
        public string UserCode 
        {
            get => userCode;
            set
            {
                userCode = value;
                OnPropertyChanged(nameof(UserCode));
            }
        }

        public string Password { get; set; }

        public LogInCommand LogInCommand { get; set; }

        public LoginVM()
        {
            LogInCommand = new(this);
        }

        internal bool LogIn()
        {
            if (int.TryParse(UserCode, out var userID))
            {
                return App.Charon.LogIn(userID, Password);
            }

            MessageBox.Show("User ID must be an integer - 5 digits.", "Invalid User ID:", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
