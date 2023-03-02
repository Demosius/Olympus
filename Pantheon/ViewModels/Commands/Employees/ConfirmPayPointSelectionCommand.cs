using System;
using System.Windows;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;


public class ConfirmPayPointSelectionCommand : ICommand
{
    public PayPointSelectionVM VM { get; set; }

    public ConfirmPayPointSelectionCommand(PayPointSelectionVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Window w) return;
        w.DialogResult = VM.SelectedPayPoint is not null;
        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}