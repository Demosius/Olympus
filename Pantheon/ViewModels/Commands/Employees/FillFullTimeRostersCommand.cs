using Pantheon.ViewModels.Pages;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModels.Commands.Employees;

public class FillFullTimeRostersCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public FillFullTimeRostersCommand(EmployeePageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.Employees.Count > 0;
    }

    public void Execute(object? parameter)
    {
        VM.FillFullTimeRosters();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}