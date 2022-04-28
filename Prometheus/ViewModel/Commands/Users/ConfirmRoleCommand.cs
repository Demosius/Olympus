using Prometheus.ViewModel.PopUp.Users;
using System;
using System.Windows;
using System.Windows.Input;

namespace Prometheus.ViewModel.Commands.Users;

internal class ConfirmRoleCommand : ICommand
{
    public SetUserRoleVM VM { get; set; }

    public ConfirmRoleCommand(SetUserRoleVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedRole is not null && VM.User?.Role is not null && VM.Charon is not null &&
               VM.SelectedRole != VM.User.Role && VM.Charon.CanAssignUserRole(VM.User.Role, VM.SelectedRole);
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Window w) throw new Exception("Window not set as parameter for Confirm Role Command.");
        if (VM.ConfirmRole()) w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}