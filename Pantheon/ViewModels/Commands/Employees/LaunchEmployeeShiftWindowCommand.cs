using Pantheon.ViewModels.Pages;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModels.Commands.Employees;

public class LaunchEmployeeShiftWindowCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public LaunchEmployeeShiftWindowCommand(EmployeePageVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon is not null && VM.SelectedEmployee is not null &&
               (VM.Charon.CanUpdateEmployee(VM.SelectedEmployee) ||
                VM.SelectedEmployee.Department is not null && VM.Charon.CanUpdateShift(VM.SelectedEmployee.Department));
    }

    public void Execute(object? parameter)
    {
        VM.LaunchEmployeeShiftWindow();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}