﻿using System;
using System.Windows.Input;

namespace Deimos.ViewModels.Commands;

public class UploadPickEventsCommand : ICommand
{
    public DeimosVM VM { get; set; }

    public UploadPickEventsCommand(DeimosVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.UploadPickEvents();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}