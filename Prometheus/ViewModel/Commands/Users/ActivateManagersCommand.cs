using System;
using System.Windows.Input;
using Prometheus.ViewModel.Pages.Users;

namespace Prometheus.ViewModel.Commands.Users;

internal class ActivateManagersCommand : ICommand
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