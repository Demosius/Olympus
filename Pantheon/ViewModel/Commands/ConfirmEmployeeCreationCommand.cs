using System;
using System.Windows;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands;

internal class ConfirmEmployeeCreationCommand : ICommand
{
    public EmployeeCreationVM VM { get; set; }

    public ConfirmEmployeeCreationCommand(EmployeeCreationVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.ValidID && VM.Role is not null;
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Window w) return;
        w.DialogResult = VM.ConfirmEmployeeCreation();
        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}