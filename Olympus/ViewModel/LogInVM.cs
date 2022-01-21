﻿using Olympus.ViewModel.Commands;
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
            LogInCommand = new LogInCommand(this);
        }

        public bool LogIn()
        {
            if (int.TryParse(UserID, out int id))
                return App.Charon.LogIn(id, Password);
            else 
                return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
