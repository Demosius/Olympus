using System;
using System.Windows;
using System.Windows.Input;
using Morpheus.ViewModels.Interfaces;

namespace Morpheus.ViewModels.Commands;

public class ConfirmAndCloseCommand : ICommand
{
    public IConfirm VM { get; set; }

    public ConfirmAndCloseCommand(IConfirm vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        var result = VM.Confirm();
        if (parameter is not Window w) return;
        w.DialogResult = result;
        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}