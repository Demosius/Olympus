using System;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

public class DeleteShiftRuleCommand : ICommand
{
    public EmployeeShiftVM VM { get; set; }

    public DeleteShiftRuleCommand(EmployeeShiftVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedRule is not null && VM.Charon.CanUpdateEmployee(VM.EmployeeVM.Employee);
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