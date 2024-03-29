﻿using System;
using System.Windows.Input;
using Pantheon.ViewModels.Pages;

namespace Pantheon.ViewModels.Commands.Rosters;

public class DeleteRosterCommand : ICommand
{
    public RosterPageVM VM { get; set; }

    public DeleteRosterCommand(RosterPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedRoster is not null;
    }

    public void Execute(object? parameter)
    {
        VM.DeleteRoster();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}