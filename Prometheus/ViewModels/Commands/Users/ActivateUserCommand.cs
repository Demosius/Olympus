using Prometheus.ViewModels.Pages.Users;
using System;
using System.Windows.Input;

namespace Prometheus.ViewModels.Commands.Users;

public class ActivateUserCommand : ICommand
{
    public UserActivateVM VM { get; set; }

    public ActivateUserCommand(UserActivateVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedEmployee is not null;
    }

    public void Execute(object? parameter)
    {
        VM.ActivateUser();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}