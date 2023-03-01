﻿using System;
using System.Windows.Input;
using Pantheon.ViewModels.Pages;

namespace Pantheon.ViewModels.Commands.Employees;

public class LaunchIconiferCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public LaunchIconiferCommand(EmployeePageVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedEmployeeVM is not null;
    }

    public void Execute(object? parameter)
    {
        VM.LaunchIconifer();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}