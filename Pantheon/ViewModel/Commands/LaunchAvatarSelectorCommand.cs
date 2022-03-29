using Pantheon.ViewModel.Pages;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands;

public class LaunchAvatarSelectorCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public LaunchAvatarSelectorCommand(EmployeePageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedEmployee is not null;
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