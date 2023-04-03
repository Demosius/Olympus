﻿using System;
using System.Windows.Input;
using Pantheon.ViewModels.Interface;

namespace Pantheon.ViewModels.Commands.Generic;

public class DeleteCommand : ICommand
{
    public ISelector VM { get; set; }

    public DeleteCommand(ISelector vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.CanDelete;
    }

    public void Execute(object? parameter)
    {
        VM.Delete();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}