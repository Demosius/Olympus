using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

public class SelectLocationCommand : ICommand
{
    public EmployeeVM VM { get; set; }

    public SelectLocationCommand(EmployeeVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon.CanCreateEmployee();
    }

    public void Execute(object? parameter)
    {
        VM.SelectLocation();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}