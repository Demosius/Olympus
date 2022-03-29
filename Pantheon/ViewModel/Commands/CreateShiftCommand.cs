﻿using Pantheon.ViewModel.Pages;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands;

internal class CreateShiftCommand : ICommand
{
    public ShiftPageVM VM { get; set; }

    public CreateShiftCommand(ShiftPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedDepartment is not null && (VM.Charon?.CanCreateShift() ?? false);
    }

    public void Execute(object? parameter)
    {
        VM.CreateShift();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}