using Prometheus.ViewModels.Pages.Users;
using System;
using System.Windows.Input;

namespace Prometheus.ViewModels.Commands.Users;

public class ActivateManagersCommand : ICommand
{
    public UserActivateVM VM { get; set; }

    public ActivateManagersCommand(UserActivateVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.CanMassCreate;
    }

    public void Execute(object? parameter)
    {
        VM.ActivateManagers();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}