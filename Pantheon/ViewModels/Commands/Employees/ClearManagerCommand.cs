using System;
using System.Windows.Input;
using Pantheon.ViewModels.Interface;

namespace Pantheon.ViewModels.Commands.Employees;

public class ClearManagerCommand : ICommand
{
    public IManagers VM { get; set; }

    public ClearManagerCommand(IManagers vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.ClearManager();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}