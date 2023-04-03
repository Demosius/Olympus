using System;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

public class CancelRuleEditCommand : ICommand
{
    public EmployeeShiftVM VM { get; set; }

    public CancelRuleEditCommand(EmployeeShiftVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.InEdit;
    }

    public void Execute(object? parameter)
    {
        VM.CancelRuleEdit();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}