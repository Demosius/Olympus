using System;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

public class EditShiftRuleCommand : ICommand
{
    public EmployeeShiftVM VM { get; set; }

    public EditShiftRuleCommand(EmployeeShiftVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedRule is not null && VM.Charon.CanUpdateEmployee(VM.Employee);
    }

    public void Execute(object? parameter)
    {
        VM.EditShiftRule();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}