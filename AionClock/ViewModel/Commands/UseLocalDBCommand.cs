﻿using System;
using System.Windows.Input;
using AionClock.ViewModel.Utility;

namespace AionClock.ViewModel.Commands;

public class UseLocalDBCommand : ICommand
{
    public DBManager Dbm { get; set; }

    public UseLocalDBCommand(DBManager dbm) { Dbm = dbm; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        Dbm.UseLocalDB();
    }
}