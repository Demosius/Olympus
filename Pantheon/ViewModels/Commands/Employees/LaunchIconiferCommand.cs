using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

public class LaunchIconiferCommand : ICommand
{
    public EmployeeVM VM { get; set; }

    public LaunchIconiferCommand(EmployeeVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.LaunchIconifer();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}