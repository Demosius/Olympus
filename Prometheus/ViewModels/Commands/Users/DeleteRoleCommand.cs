using Prometheus.ViewModels.Pages.Users;
using System;
using System.Windows.Input;

namespace Prometheus.ViewModels.Commands.Users;

public class DeleteRoleCommand : ICommand
{
    public RolesVM VM { get; set; }

    public DeleteRoleCommand(RolesVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedRole is not null &&
               VM.SelectedRole.Users.Count == 0 &&
               VM.Charon.CanDeleteUserRole();
    }

    public void Execute(object? parameter)
    {
        _ = VM.DeleteRole();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}