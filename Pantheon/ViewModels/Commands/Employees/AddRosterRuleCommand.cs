using System;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

public class AddRosterRuleCommand : ICommand
{
    public EmployeeShiftVM VM { get; set; }

    public AddRosterRuleCommand(EmployeeShiftVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.RosterRule is not null && VM.Charon.CanUpdateEmployee(VM.Employee) && VM.RosterRule.IsValid();
    }

    public void Execute(object? parameter)
    {
        VM.AddRosterRule();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}