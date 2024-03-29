﻿using System;
using System.Windows.Input;
using Pantheon.ViewModels.Pages;

namespace Pantheon.ViewModels.Commands.Employees;

public class DeleteEmployeeCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public DeleteEmployeeCommand(EmployeePageVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedEmployee is not null && (VM.Charon?.CanDeleteEmployee(VM.SelectedEmployee) ?? false);
    }

    public void Execute(object? parameter)
    {
        VM.DeleteEmployee();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}