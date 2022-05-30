using Pantheon.ViewModels.PopUp.Employees;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModels.Commands.Employees;

internal class DeleteShiftRuleCommand : ICommand
{
    public EmployeeShiftVM VM { get; set; }

    public DeleteShiftRuleCommand(EmployeeShiftVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon is not null && VM.Employee is not null &&
               VM.SelectedRule is not null && VM.Charon.CanUpdateEmployee(VM.Employee);
    }

    public void Execute(object? parameter)
    {
        VM.DeleteShiftRule();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}