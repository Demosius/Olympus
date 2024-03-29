﻿using System;
using System.Windows.Input;
using Pantheon.ViewModels.Pages;

namespace Pantheon.ViewModels.Commands.Employees;

public class CreateNewEmployeeCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public CreateNewEmployeeCommand(EmployeePageVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon?.CanCreateEmployee() ?? false;
    }

    public void Execute(object? parameter)
    {
        VM.CreateNewEmployee();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}