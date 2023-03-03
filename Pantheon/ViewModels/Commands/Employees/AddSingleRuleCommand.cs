using System;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

public class AddSingleRuleCommand : ICommand
{
    public EmployeeShiftVM VM { get; set; }

    public AddSingleRuleCommand(EmployeeShiftVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SingleRule is not null && VM.Charon.CanUpdateEmployee(VM.Employee) && VM.SingleRule.IsValid();
    }

    public void Execute(object? parameter)
    {
        VM.AddSingleRule();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}