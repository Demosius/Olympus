﻿using Olympus.ViewModels.Utility;
using System;
using System.Windows.Input;

namespace Olympus.ViewModels.Commands;

public class MoveDatabaseCommand : ICommand
{
    public DBManager VM { get; set; }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public MoveDatabaseCommand(DBManager vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return (parameter ?? "null") is string dbLocation && App.Charon.CanMoveDatabase() && dbLocation.ToLower() != "local";
    }

    public async void Execute(object? parameter)
    {
        await VM.MoveDatabase();
    }
}