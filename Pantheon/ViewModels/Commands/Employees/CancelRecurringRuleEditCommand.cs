using Pantheon.ViewModels.PopUp.Employees;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModels.Commands.Employees;

internal class CancelRecurringRuleEditCommand : ICommand
{
    public EmployeeShiftVM VM { get; set; }

    public CancelRecurringRuleEditCommand(EmployeeShiftVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.RecurringRule?.InEdit ?? false;
    }

    public void Execute(object? parameter)
    {
        VM.CancelRecurringRuleEdit();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}