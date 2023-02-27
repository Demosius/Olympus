using System;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

internal class CancelRosterRuleEditCommand : ICommand
{
    public EmployeeShiftVM VM { get; set; }

    public CancelRosterRuleEditCommand(EmployeeShiftVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.RosterRule?.InEdit ?? false;
    }

    public void Execute(object? parameter)
    {
        VM.CancelRosterRuleEdit();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}