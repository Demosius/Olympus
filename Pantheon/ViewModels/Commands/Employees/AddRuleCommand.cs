using System;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

public class AddRuleCommand : ICommand
{
    public EmployeeShiftVM VM { get; set; }

    public AddRuleCommand(EmployeeShiftVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.Rule is not null && VM.Charon.CanUpdateEmployee(VM.Employee) && VM.Rule.IsValid;
    }

    public void Execute(object? parameter)
    {
        VM.AddRule();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}