using System;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

public class AddRecurringRuleCommand : ICommand
{
    public EmployeeShiftVM VM { get; set; }

    public AddRecurringRuleCommand(EmployeeShiftVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.RecurringRule is not null && VM.Charon.CanUpdateEmployee(VM.Employee) && VM.RecurringRule.IsValid();
    }

    public void Execute(object? parameter)
    {
        VM.AddRecurringRule();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}