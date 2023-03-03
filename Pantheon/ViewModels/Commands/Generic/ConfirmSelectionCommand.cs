using System;
using System.Windows;
using System.Windows.Input;
using Pantheon.ViewModels.Interface;

namespace Pantheon.ViewModels.Commands.Generic;

public class ConfirmSelectionCommand : ICommand
{
    public ISelector VM { get; set; }

    public ConfirmSelectionCommand(ISelector vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.CanConfirm;
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Window w) return;
        w.DialogResult = VM.CanConfirm;
        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}