using System;
using System.Windows.Input;
using Pantheon.ViewModels.Interface;

namespace Pantheon.ViewModels.Commands.Employees;


public class SelectManagerCommand : ICommand
{
    public IManagers VM { get; set; }

    public SelectManagerCommand(IManagers vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.SelectManager();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}