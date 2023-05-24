using Prometheus.ViewModels.Pages.Users;
using System;
using System.Windows.Input;

namespace Prometheus.ViewModels.Commands.Users;

public class DeactivateUserCommand : ICommand
{
    public UserViewVM VM { get; set; }

    public DeactivateUserCommand(UserViewVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedUser?.Employee is not null &&
               VM.Charon.CanDeleteUser(VM.SelectedUser.Employee);
    }

    public async void Execute(object? parameter)
    {
        await VM.DeactivateUser();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}