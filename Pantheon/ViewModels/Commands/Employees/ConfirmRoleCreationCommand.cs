using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

internal class ConfirmRoleCreationCommand : ICommand
{
    public RoleCreationVM VM { get; set; }

    public ConfirmRoleCreationCommand(RoleCreationVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.Role.Name is not null or "" && VM.Role.Department is not null && !VM.Roles.Select(r => r.Name).Contains(VM.Role.Name);
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Window w) return;
        w.DialogResult = VM.ConfirmRoleCreation();
        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}