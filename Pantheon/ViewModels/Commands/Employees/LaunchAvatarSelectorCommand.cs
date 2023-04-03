using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.Pages;

namespace Pantheon.ViewModels.Commands.Employees;

public class LaunchAvatarSelectorCommand : ICommand
{
    public EmployeeVM VM { get; set; }

    public LaunchAvatarSelectorCommand(EmployeeVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.LaunchAvatarSelector();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}