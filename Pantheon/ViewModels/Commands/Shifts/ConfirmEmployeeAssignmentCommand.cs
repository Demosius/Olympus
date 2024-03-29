﻿using System;
using System.Windows;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Shifts;

namespace Pantheon.ViewModels.Commands.Shifts;

public class ConfirmEmployeeAssignmentCommand : ICommand
{
    public ShiftEmployeeVM VM { get; set; }

    public ConfirmEmployeeAssignmentCommand(ShiftEmployeeVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        var w = parameter as Window;
        VM.ConfirmEmployeeAssignment();
        w?.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}