using Pantheon.ViewModels.Pages;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModels.Commands.Employees;

public class AddLocationCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public AddLocationCommand(EmployeePageVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon?.CanCreateEmployee() ?? false;
    }

    public void Execute(object? parameter)
    {
        VM.AddLocation();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}