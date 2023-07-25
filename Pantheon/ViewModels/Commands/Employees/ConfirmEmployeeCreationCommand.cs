using System;
using System.Windows;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

public class ConfirmEmployeeCreationCommand : ICommand
{
    public EmployeeCreationVM VM { get; set; }

    public ConfirmEmployeeCreationCommand(EmployeeCreationVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.ValidID is null || VM.ValidID == true && VM.SelectedRole is not null;
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