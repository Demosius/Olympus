﻿using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using Pantheon.ViewModels.Commands;

namespace Pantheon.ViewModels;

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
        LogInCommand = new LogInCommand(this);
        userCode = "";
        Password = "";
    }

    internal async Task<bool> LogIn()
    {
        if (int.TryParse(UserCode, out var userID))
        {
            if (await App.Charon.LogInAsync(userID, Password))
                return true;

            MessageBox.Show("Incorrect User ID and Password combination.", "Invalid Log In", MessageBoxButton.OK,
                MessageBoxImage.Error);
            return false;
        }

        MessageBox.Show("User ID must be an integer - 5 digits.", "Invalid User ID:", MessageBoxButton.OK, MessageBoxImage.Error);
        return false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}