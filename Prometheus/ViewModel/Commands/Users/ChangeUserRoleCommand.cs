using System;
using System.Windows.Input;
using Prometheus.ViewModel.Pages.Users;

namespace Prometheus.ViewModel.Commands.Users;

internal class ChangeUserRoleCommand : ICommand
{
    public UserViewVM VM { get; set; }

    public ChangeUserRoleCommand(UserViewVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedUser is not null && VM.Charon is not null && VM.Charon.CanAssignUserRole();
    }

    public void Execute(object? parameter)
    {
        VM.ChangeUserRole();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}