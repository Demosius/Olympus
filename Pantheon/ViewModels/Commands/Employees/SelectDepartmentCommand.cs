using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

public class SelectDepartmentCommand : ICommand
{
    public EmployeeVM VM { get; set; }

    public SelectDepartmentCommand(EmployeeVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon.CanCreateDepartment();
    }

    public void Execute(object? parameter)
    {
        VM.SelectDepartment();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}