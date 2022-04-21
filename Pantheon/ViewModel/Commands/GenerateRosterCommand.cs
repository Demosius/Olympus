﻿using System;
using System.Windows.Input;
using Pantheon.ViewModel.Controls;
using Pantheon.ViewModel.Pages;

namespace Pantheon.ViewModel.Commands;

internal class GenerateRosterCommand : ICommand
{
    public DepartmentRosterVM VM { get; set; }

    public GenerateRosterCommand(DepartmentRosterVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.IsInitialized;
    }

    public void Execute(object? parameter)
    {
        VM.GenerateRosterAssignments();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}