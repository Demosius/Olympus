using System;
using System.Windows.Input;
using Pantheon.ViewModels.Interface;

namespace Pantheon.ViewModels.Commands.Employees;

public class ClearDepartmentCommand : ICommand
{
    public IDepartments VM { get; set; }

    public ClearDepartmentCommand(IDepartments vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.ClearDepartment();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}