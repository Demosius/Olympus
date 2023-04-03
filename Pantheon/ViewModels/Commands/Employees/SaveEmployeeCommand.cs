using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

public class SaveEmployeeCommand : ICommand
{
    public EmployeeVM VM { get; set; }

    public SaveEmployeeCommand(EmployeeVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon.CanUpdateEmployee(VM.Employee);
    }

    public void Execute(object? parameter)
    {
        VM.SaveEmployee();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}