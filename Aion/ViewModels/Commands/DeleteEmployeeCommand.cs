using System;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;


public class DeleteEmployeeCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public DeleteEmployeeCommand(EmployeePageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        EmployeePageVM.DeleteEmployee();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}