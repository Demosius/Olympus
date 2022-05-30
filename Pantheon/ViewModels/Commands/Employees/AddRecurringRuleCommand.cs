using Pantheon.ViewModels.PopUp.Employees;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModels.Commands.Employees;

internal class AddRecurringRuleCommand : ICommand
{
    public EmployeeShiftVM VM { get; set; }

    public AddRecurringRuleCommand(EmployeeShiftVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon is not null && VM.Employee is not null && VM.RecurringRule is not null &&
            VM.Charon.CanUpdateEmployee(VM.Employee) && VM.RecurringRule.IsValid();
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