﻿using System;
using System.Windows.Input;
using Olympus.ViewModels.Utility;

namespace Olympus.ViewModels.Commands;

public class MergeDatabaseCommand : ICommand
{
    public DBManager DBM { get; set; }

    public MergeDatabaseCommand(DBManager dbm) { DBM = dbm; }

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
        DBManager.MergeDatabase();
    }
}